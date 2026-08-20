using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Dtos;
using UniversitySystem3.Models;
using UniversitySystem3.Repositories;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Teachers;

public class StudentCourseManagementService : IStudentCourseManagementService
{
    private readonly IUnitOfWork _uow;

    public StudentCourseManagementService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private async Task<int?> GetEmployeeMajorId(int employeeId)
    {
        var employee = await _uow.Employees.GetByIdAsync(employeeId);
        return employee?.MajorId;
    }
    private async Task<Term?> GetActiveTerm()
    {
        return await _uow.Terms.Query().FirstOrDefaultAsync(t => t.Status == "فعال");
    }

    private async Task<Term?> GetPreviousTerm(Term currentTerm)
    {
        if (currentTerm.StartDate == null)
            return null;

        return await _uow.Terms.Query()
            .Where(e => e.StartDate != null && e.StartDate < currentTerm.StartDate)
            .FirstOrDefaultAsync();
    }

    private async Task<double> CalculateTermAverage(int studentId, int termId)
    {
        var courseRegs = await _uow.CourseRegs.Query()
              .Include(e => e.Class)
              .Where(e => e.StudentId == studentId
              && e.TermId == termId
              && e.Score != null
              && e.Class != null).ToListAsync();

        int totalCredits = courseRegs.Sum(e => e.Class!.Credit);
        if (totalCredits == 0)
            return 0;

        double weightedSum = courseRegs.Sum(e => (e.Score ?? 0) * e.Class!.Credit);
        return weightedSum / totalCredits;
    }

    public async Task<ServiceResult<object>> GetStudentsAsync(int currentEmployeeId, string? search)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var query = _uow.Students.Query()
            .Include(e => e.Major)
            .Include(e => e.EnteranceYear)
            .Where(e => e.MajorId == majorId).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                (s.FullName != null && s.FullName.Contains(search)) ||
                (s.StudentCode != null && s.StudentCode.Contains(search)));
        }

        var student = await query.ToListAsync();

        var grouped = student
           .GroupBy(s => s.EnteranceYear?.EnteranceYear)
           .OrderBy(g => g.Key)
           .Select(g => new StudentYearGroupDto
           {
               EnteranceYear = g.Key?.ToString() ?? "نامشخص",
               Students = g.Select(s => new StudentListItemDto
               {
                   StudentId = s.StudentId,
                   FullName = s.FullName ?? "",
                   StudentCode = s.StudentCode ?? "",
                   MajorName = s.Major?.MajorName ?? ""
               }).ToList()
           })
           .ToList();

        return ServiceResult<object>.Ok(grouped);
    }

    public async Task<ServiceResult<StudentCourseSelectionDto>> GetStudentCourseSelectionAsync(int currentEmployeeId, int studentId)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<StudentCourseSelectionDto>.Fail
                ("رشته‌ی این کارمند آموزش مشخص نیست.");

        var student = await _uow.Students.Query()
            .Include(s => s.EnteranceYear)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);

        if (student == null)
            return ServiceResult<StudentCourseSelectionDto>.Fail
                ("دانشجو پیدا نشد.", ServiceResultType.NotFound);

        if (student.MajorId != majorId)
            return ServiceResult<StudentCourseSelectionDto>.Fail
                ("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

        var activeTerm = await GetActiveTerm();
        if (activeTerm == null)
            return ServiceResult<StudentCourseSelectionDto>.Fail
                ("در حال حاضر هیچ ترم فعالی تعریف نشده است.");

        var previousTerm = await GetPreviousTerm(activeTerm);

        double previousTermAverage = previousTerm != null
            ? await CalculateTermAverage(studentId, previousTerm.TermId) : 0;

        var selectedClasses = await _uow.CourseRegs.Query()
            .Include(cr => cr.Class)
                .ThenInclude(c => c!.Lesson)
            .Include(cr => cr.Class)
                .ThenInclude(c => c!.Employee)
            .Include(cr => cr.Class)
                .ThenInclude(c => c!.Major)
            .Where(cr => cr.StudentId == studentId && cr.TermId == activeTerm.TermId)
            .Select(cr => new SelectedClassDto
            {
                CourseRegId = cr.CourseRegId,
                ClassCode = cr.Class!.ClassCode,
                LessonTitle = cr.Class.Lesson!.LessonTitle,
                EmployeeFullName = cr.Class.Employee!.FullName ?? string.Empty,
                MajorName = cr.Class.Major!.MajorName ?? string.Empty
            })
            .ToListAsync();

        var result = new StudentCourseSelectionDto
        {
            FullName = student.FullName ?? string.Empty,
            StudentCode = student.StudentCode ?? string.Empty,
            EnteranceYearTitle = student.EnteranceYear?.EnteranceYear?.ToString() ?? string.Empty,
            PreviousTermAverage = previousTermAverage,
            SelectedClasses = selectedClasses
        };

        return ServiceResult<StudentCourseSelectionDto>.Ok(result);
    }
    public async Task<ServiceResult<object>> DeleteCourseRegAsync(int currentEmployeeId, int courseRegId)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var courseReg = await _uow.CourseRegs.Query()
            .Include(e => e.Student)
            .FirstOrDefaultAsync(e => e.CourseRegId == majorId);

        if (courseReg == null)
            return ServiceResult<object>.Fail("این ثبت نام پیدا نشد", ServiceResultType.NotFound);

        if (courseReg.Student?.MajorId != majorId)
            return ServiceResult<object>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

        _uow.CourseRegs.Remove(courseReg);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "درس با موفقیت حذف شد." });



    }
}
