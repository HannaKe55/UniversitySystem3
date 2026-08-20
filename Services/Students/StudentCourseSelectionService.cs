using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Dtos;
using UniversitySystem3.Models;
using UniversitySystem3.Repositories;
using UniversitySystem3.Services.Common;
namespace UniversitySystem3.Services.Students;

public class StudentCourseSelectionService : IStudentCourseSelectionServicecs
{
    private readonly IUnitOfWork _uow;

    public StudentCourseSelectionService(IUnitOfWork uow)
        {
           
         _uow = uow;
        }

    private IQueryable<UniversitySystem3.Models.Class> GetVisibleClassesQuery(Student student)
    {
        return _uow.Classes.Query()
              .Include(e => e.Lesson)
              .Include(e => e.Employee)
              .Include(e => e.Major)
              .Include(e => e.LessonType)
              .Include(e => e.CourseRegs)
              .Where(e => e.MajorId == student.MajorId &&
                           e.ForEnteranceYearId == student.EnteranceYearId &&
                           e.TermId == student.CurrentTermId);

    }

    public async Task<ServiceResult<object>> GetAvailableClassesAsync(int studentId, string? search,
      List<int>? teacherIds, List<int>? lessonTypeIds, List<int>? lessonIds, List<int>? credits)
    {
        var student = await _uow.Students.GetByIdAsync(studentId);
        if (student == null)
            return ServiceResult<object>.Fail("اطلاعات دانشجو پیدا نشد", ServiceResultType.NotFound);

        if (student.MajorId == null || student.EnteranceYearId == null || student.CurrentTermId == null)
            return ServiceResult<object>.Fail("اطلاعات رشته، سال ورودی یا ترم جاری دانشجو کامل نیست.");

        var query = GetVisibleClassesQuery(student);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.Lesson!.LessonTitle.Contains(search) ||
                c.Employee!.FullName!.Contains(search));
        }

        if (teacherIds != null && teacherIds.Count > 0)
            query = query.Where(e => teacherIds.Contains(e.EmployeeId ?? 0));

        if (lessonTypeIds != null && lessonTypeIds.Count > 0)
            query = query.Where(e => lessonTypeIds.Contains(e.LessonTypeId ?? 0));

        if (lessonIds != null && lessonIds.Count > 0)
            query = query.Where(e => lessonIds.Contains(e.LessonId ?? 0));

        if (credits != null && credits.Count > 0)
            query = query.Where(e => credits.Contains(e.Credit));

        var result = await query.Select(e => new AvailableClassesDto
        {
            ClassId = e.ClassId,
            ClassCode = e.ClassCode,
            LessonTitle = e.Lesson!.LessonTitle,
            EmployeeFullName = e.Employee!.FullName ?? string.Empty,
            MajorName = e.Major!.MajorName ?? string.Empty,
            Capacity = e.Capacity ?? 0,
            RegisteredCount = e.CourseRegs.Count,
            LessonTypeTitle = e.LessonType!.LessonTypeTitle ?? string.Empty,
            Credit = e.Credit,

        }).ToListAsync();


        return ServiceResult<object>.Ok(result);

    }
    // Filtering professors (student course registration)
    public async Task<ServiceResult<object>> GetFilterTeachersAsync(int studentId)
    {
        var student = await _uow.Students.GetByIdAsync(studentId);
        if (student == null)
            return ServiceResult<object>.Fail("اطلاعات دانشجو پیدا نشد", ServiceResultType.NotFound);

        var teachers = await GetVisibleClassesQuery(student)
            .Select(e => new { e.Employee!.EmployeeId, e.Employee.FullName })
            .Distinct().ToListAsync();

        return ServiceResult<object>.Ok(teachers);
    }

    // Filtering Lesson Types (student course registration)
    public async Task<ServiceResult<object>> GetFilterLessonTypesAsync(int studentId)
    {
        var student = await _uow.Students.GetByIdAsync(studentId);
        if (student == null)
            return ServiceResult<object>.Fail("اطلاعات دانشجو پیدا نشد", ServiceResultType.NotFound);

        var lessonTypes = await GetVisibleClassesQuery(student)
            .Select(e => new { e.LessonType!.LessonTypeId, e.LessonType!.LessonTypeTitle })
            .Distinct().ToListAsync();

        return ServiceResult<object>.Ok(lessonTypes);
    }

    // Filtering Lessons (student course registration)
    public async Task<ServiceResult<object>> GetFilterLessonsAsync(int studentId)
    {

        var student = await _uow.Students.GetByIdAsync(studentId);
        if (student == null)
            return ServiceResult<object>.Fail("اطلاعات دانشجو پیدا نشد", ServiceResultType.NotFound);

        var lessons = await GetVisibleClassesQuery(student)
            .Select(e => new { e.Lesson.LessonId, e.Lesson.LessonTitle })
            .Distinct().ToListAsync();

        return ServiceResult<object>.Ok(lessons);
    }

    // Filtering lesson credit
    public async Task<ServiceResult<object>> GetFilterCreditsAsync(int studentId)
    {

        var student = await _uow.Students.GetByIdAsync(studentId);
        if (student == null)
            return ServiceResult<object>.Fail("اطلاعات دانشجو پیدا نشد", ServiceResultType.NotFound);

        var credits = await GetVisibleClassesQuery(student)
            .Select(e => e.Credit)
            .Distinct()
            .OrderBy(e => e)
            .ToListAsync();

        return ServiceResult<object>.Ok(credits);
    }

    public async Task<ServiceResult<object>> RegisterCoursesAsync(int studentId, RegisterCourseDto dto)
    {

        var student = await _uow.Students.GetByIdAsync(studentId);
        if (student == null)
            return ServiceResult<object>.Fail("اطلاعات دانشجو پیدا نشد", ServiceResultType.NotFound);

        if (dto.ClassIds == null || dto.ClassIds.Count == 0)
            return ServiceResult<object>.Fail("هیچ درسی برای انتخاب واحد انتخاب شده است.");

        var defaultStatus = await _uow.Statuses.Query()
            .FirstOrDefaultAsync(e => e.StatusTitle == "در حال انتخاب واحد");

        if (defaultStatus == null)
            return ServiceResult<object>.Fail("وضعیت پیش‌فرض ثبت‌نام در سیستم تعریف نشده است.");

        var results = new List<object>();

        foreach (var classId in dto.ClassIds)
        {
            var targetClass = await _uow.Classes.Query()
                .Include(e => e.CourseRegs)
                .FirstOrDefaultAsync(e => e.ClassId == classId);

            if (targetClass == null)
            {
                results.Add(new { classId, success = "false", message = "کلاس پیدا نشد" });
                continue;
            }

            if (targetClass.MajorId != student.MajorId ||
                targetClass.ForEnteranceYearId != student.EnteranceYearId ||
                targetClass.TermId != student.CurrentTermId)
            {
                results.Add(new { classId, success = false, message = "این کلاس برای شما قابل انتخاب نیست." });
                continue;
            }

            bool alreadyRegistered = targetClass.CourseRegs.Any(e => e.StudentId == student.StudentId);
            if (alreadyRegistered)
            {
                results.Add(new { classId, success = false, message = "قبلاً برای این درس ثبت‌نام کرده‌اید." });
                continue;
            }

            if (targetClass.CourseRegs.Count >= (targetClass.Capacity ?? 0))
            {
                results.Add(new { classId, success = false, message = "ظرفیت این کلاس تکمیل شده است." });
                continue;
            }

            var courseReg = new CourseReg
            {
                StudentId = student.StudentId,
                ClassId = classId,
                EmployeeId = targetClass.EmployeeId,
                TermId = targetClass.TermId,
                StatusId = defaultStatus.StatusId
            };

            await _uow.CourseRegs.AddAsync(courseReg);
            results.Add(new { classId, success = true, message = "با موفقیت ثبت شد." });

        }

        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "عملیات ثبت دروس انجام شد.", details = results });
    }

}
