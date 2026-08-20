using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UniversitySystem3.Dtos;
using UniversitySystem3.Models;
using UniversitySystem3.Repositories;
using UniversitySystem3.Services.Common;
using UniversitySystem3.Services.Students;

namespace UniversitySystem3.Services.Students;

public class StudentInfoService : IStudentInfoService
{
    private readonly IUnitOfWork _uow;
    public StudentInfoService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ServiceResult<StudentSummaryDto>> GetSummaryAsync(int studentId)
    {
        var student = await _uow.Students.Query()
             .Include(e => e.Major)
             .Include(e => e.EnteranceYear)
             .FirstOrDefaultAsync(e => e.StudentId == studentId);

        if (student == null)
            return ServiceResult<StudentSummaryDto>.Fail
                ("اطلاعات دانشجو پدا نشد", ServiceResultType.NotFound);

        var allCourseRegs = await _uow.CourseRegs.Query()
            .Include(e => e.Class)
              .ThenInclude(c => c!.Term)
            .Include(e => e.Status)
            .Where(e => e.StudentId == studentId).ToListAsync();

        var termGroups = allCourseRegs
            .Where(e => e.ClassId != null && e.Class.TermId != null)
            .GroupBy(e => e.Class!.TermId)
            .ToList();

        var termSummries = termGroups
            .Select(group => CalculateTermSummary(group.ToList())).ToList();

        int totalPassedCredits = termSummries.Sum(e => e.PassedCredits);
        double overallAverage = CalculateWeightedAverage(allCourseRegs);

        var result = new StudentSummaryDto
        {
            FullName = student.FullName ?? string.Empty,
            StudentCode = student.StudentCode ?? string.Empty,
            MajorName = student.Major?.MajorName ?? string.Empty,
            EnteranceYearTitle = student.EnteranceYear?.EnteranceYear?.ToString()
            ?? string.Empty,

            TotalPassedCredits = totalPassedCredits,
            OverallAverage = Math.Round(overallAverage, 2),
            Terms = termSummries
        };

        return ServiceResult<StudentSummaryDto>.Ok(result);
    }

    public async Task<ServiceResult<TermDetailDto>> GetTermDetailAsync(int studentId, int termId)
    {
        var term = await _uow.Terms.GetByIdAsync(termId);
        if (term == null)
            return ServiceResult<TermDetailDto>.Fail("ترم مورد نظر پیدا نشد", ServiceResultType.NotFound);

        var courseRegs = await _uow.CourseRegs.Query()
            .Include(e => e.Class)
              .ThenInclude(e => e!.Lesson)
            .Include(e => e.Class)
              .ThenInclude(e => e!.LessonType)
            .Include(e => e.Status)
            .Where(e => e.StudentId == studentId && e.TermId == termId)
            .ToListAsync();

        if (courseRegs.Count == 0)
            return ServiceResult<TermDetailDto>.Fail("هیچ درسی برای این ترم ثبت نشده است.");

        var courseDetails = courseRegs.Select(e => new CourseDetailDto
        {
            ClassCode = e.Class?.ClassCode ?? string.Empty,
            LessonTitle = e.Class?.Lesson?.LessonTitle ?? string.Empty,
            Credit = e.Class?.Credit ?? 0,
            Score = e.Score,
            ResultText = e.Status?.StatusTitle ?? string.Empty,
            LessonTypeTitle = e.Class?.LessonType?.LessonTypeTitle ?? string.Empty,
        }).ToList();

        var summary = CalculateTermSummary(courseRegs);

        var result = new TermDetailDto
        {
            TermCode = term.TermCode,
            Courses = courseDetails,
            TakenCredits = summary.TakenCredits,
            PassedCredits = summary.PassedCredits,
            FailedCredits = summary.FailedCredits,
            TermAverage = summary.TermAverage,
            ProbationStatus = summary.TermAverage < 12 ? "مشروط" : "-----"
        };

        return ServiceResult<TermDetailDto>.Ok(result);
    }

    private TermSummaryDto CalculateTermSummary(List<CourseReg> courseRegs)
    {
        var firstClass = courseRegs.FirstOrDefault()!.Class;
        int takenCredits = courseRegs.Sum(e => e.Class?.Credit ?? 0);

        int passedCredits = courseRegs
            .Where(e => e.Status?.StatusTitle == "قبول شده")
            .Sum(e => e.Class?.Credit ?? 0);

        int failedCredits = courseRegs
           .Where(e => e.Status?.StatusTitle == "رد شده")
           .Sum(e => e.Class?.Credit ?? 0);

        double average = CalculateWeightedAverage(courseRegs);

        return new TermSummaryDto
        {

            TermId = firstClass?.TermId ?? 0,
            TermCode = firstClass?.Term?.TermCode ?? string.Empty,
            TakenCredits = takenCredits,
            PassedCredits = passedCredits,
            FailedCredits = failedCredits,
            TermAverage = Math.Round(average, 2)
        };
    }

    private double CalculateWeightedAverage(List<CourseReg> courseRegs)
    {
        var scoredCourses = courseRegs
              .Where(e => e.Score != null && e.Class != null)
              .ToList();

        int totalCredits = scoredCourses.Sum(e => e.Class!.Credit);

        if (totalCredits == 0)
            return 0;

        double weightedSum = scoredCourses.Sum(e => (e.Score ?? 0) * e.Class!.Credit);
        return weightedSum / totalCredits;

    }

}
