using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Common.Exceptions;
using UniversitySystem3.Dtos;
using UniversitySystem3.Models;
using UniversitySystem3.Repositories;
using UniversitySystem3.Services.Common;


namespace UniversitySystem3.Services.Class;

public class ClassService : IClassService
{
    private readonly IUnitOfWork _uow;

    public ClassService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private async Task<int?> GetEmployeeMajorId(int employeeId)
    {
        var employee = await _uow.Employees.GetByIdAsync(employeeId);
        return employee?.MajorId;
    }

    // Data for drop-downs
    public async Task<ServiceResult<object>> GetLessonsAsync()
    { 
        var lesson = await _uow.Lessons.Query()
            .Select(e => new { e.LessonId , e.LessonTitle }).ToListAsync();
        return ServiceResult<object>.Ok(lesson);
    }

    public async Task<ServiceResult<object>> GetLocationsAsync()
    {
        var lovations = _uow.ClassLocations.Query()
             .Select(e => new { e.Id, e.LocationName }).ToListAsync();
        return ServiceResult<object>.Ok(lovations);
    }

    public async Task<ServiceResult<object>> GetEntranceYearsAsync()
    {
        var entranceYears = await _uow.Terms.Query()
                .Where(t => t.EnteranceYear != null)
                .Select(t => new { t.TermId, t.EnteranceYear })
                .ToListAsync();

        return ServiceResult<object>.Ok(entranceYears);

    }

    public async Task<ServiceResult<object>> GetLessonTypesAsync()
    {
        var lessonTypes = await _uow.LessonTypes.Query()
            .Select(lt => new { lt.LessonTypeId, lt.LessonTypeTitle })
            .ToListAsync();

        return ServiceResult<object>.Ok(lessonTypes);
    }

    public async Task<Lesson> ResolveLesson(int? lessonId, string? newLessonTitle)
    {
        if (lessonId.HasValue)
        {
            var existingLesson = await  _uow.Lessons.GetByIdAsync(lessonId.Value)
                ?? throw new NotFoundException("درس انتخاب شده معتبر نیست.");

            return existingLesson;
        }

        if(!string.IsNullOrWhiteSpace(newLessonTitle))
        {
            var lesson = await _uow.Lessons.Query()
                            .FirstOrDefaultAsync(l => l.LessonTitle == newLessonTitle);

            if (lesson == null)
            {
                lesson = new Lesson { LessonTitle = newLessonTitle, LessonCode = 0 };
                await _uow.Lessons.AddAsync(lesson);
                await _uow.SaveChangesAsync();
            }

            return lesson;
        }
        throw new BadRequestException("باید یک درس را از لیست انتخاب کنید یا عنوان درس جدید را وارد کنید.");
    
    }

    public async Task<ServiceResult<object>> CreateClassAsync(int currentEmployeeId, CreateClassDto dto)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        bool codeExists = await _uow.Classes.Query().AnyAsync(c => c.ClassCode == dto.ClassCode);
        if (codeExists)
            throw new ConflictException("این کد کلاس قبلاً ثبت شده است.");

        var lesson = await ResolveLesson(dto.LessonId, dto.NewLessonTitle);

        var teacher = await _uow.Employees.Query()
            .FirstOrDefaultAsync(e => e.EmployeeId == dto.EmployeeId && e.RoleId == 2)
            ?? throw new NotFoundException("استاد انتخاب‌شده معتبر نیست.");

        if (teacher.MajorId != majorId)
            return ServiceResult<object>.Fail("استاد انتخاب‌شده مربوط به رشته‌ی شما نیست.");

        bool locationExists = await _uow.ClassLocations.Query()
            .AnyAsync(l => l.Id == dto.ClassLocationId);

        if (!locationExists)
            return ServiceResult<object>.Fail("محل برگزاری انتخاب‌شده معتبر نیست.");

        bool termExists = await _uow.Terms.Query().AnyAsync(t => t.TermId == dto.TermId);
        if (!termExists)
            return ServiceResult<object>.Fail("ترم انتخاب‌شده معتبر نیست.");

        bool entranceYearExists = await _uow.Terms.Query()
            .AnyAsync(t => t.TermId == dto.ForEnteranceYearId && t.EnteranceYear != null);
        if (!entranceYearExists)
            return ServiceResult<object>.Fail("سال ورودی انتخاب‌شده معتبر نیست.");

        bool lessonTypeExists = await _uow.LessonTypes.Query()
            .AnyAsync(lt => lt.LessonTypeId == dto.LessonTypeId);

        if (!lessonTypeExists)
            return ServiceResult<object>.Fail("نوع درس انتخاب‌شده معتبر نیست.");

        var newClass = new Models.Class
        {
            ClassCode = dto.ClassCode,
            LessonId = lesson.LessonId,
            EmployeeId = dto.EmployeeId,
            Capacity = dto.Capacity,
            ClassLocationId = dto.ClassLocationId,
            MajorId = majorId,
            FinalExamDate = dto.FinalExamDate,
            TermId = dto.TermId,
            ForEnteranceYearId = dto.ForEnteranceYearId,
            DateTime = dto.ClassSchedule,
            LessonTypeId = dto.LessonTypeId,
            Credit = dto.Credit
        };

        await _uow.Classes.AddAsync(newClass);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "کلاس با موفقیت ثبت شد.", classId = newClass.ClassId });
    }

    public async Task<ServiceResult<object>> GetClassListAsync(int currentEmployeeId)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var classes = await _uow.Classes.Query()
            .Include(e => e.Lesson)
            .Include(e => e.Employee)
            .Include(e => e.Major)
            .Where(e => e.MajorId == majorId)
            .Select(e => new ClassListItemDto
            {
                ClassId = e.ClassId,
                ClassCode = e.ClassCode,
                LessonTitle = e.Lesson!.LessonTitle,
                EmployeeFullName = e.Employee!.FullName ?? string.Empty,
                MajorName = e.Major!.MajorName ?? string.Empty
            }).ToListAsync();

        return ServiceResult<object>.Ok(classes);
    }

    public async Task<ServiceResult<ClassDetailDto>> GetClassByIdAsync(int currentEmployeeId , int classId)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<ClassDetailDto>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var c = await _uow.Classes.Query()
            .Include(e => e.Lesson)
            .Include(e => e.Employee)
            .Include(e => e.Major)
            .Include(e => e.ClassLocation)
            .Include(e => e.ForEnteranceYear)
            .Include(e => e.LessonType)
            .FirstOrDefaultAsync(e => e.ClassId == classId)
            ?? throw new NotFoundException("کلاس موردنظر یافت نشد.");

        if (c.MajorId != majorId)
            throw new ForbiddenException();

        var dto = new ClassDetailDto
        {
            ClassId = c.ClassId,
            ClassCode = c.ClassCode,
            LessonId = c.LessonId ?? 0,
            LessonTitle = c.Lesson?.LessonTitle ?? string.Empty,
            EmployeeId = c.EmployeeId ?? 0,
            EmployeeFullName = c.Employee?.FullName ?? string.Empty,
            Capacity = c.Capacity ?? 0,
            ClassLocationId = c.ClassLocation?.Id ?? 0,
            ClassLocationName = c.ClassLocation?.LocationName ?? string.Empty,
            MajorId = c.MajorId ?? 0,
            MajorName = c.Major?.MajorName ?? string.Empty,
            FinalExamDate = c.FinalExamDate ?? default,
            TermId = c.TermId ?? 0,
            ForEnteranceYearId = c.ForEnteranceYearId ?? 0,
            ForEnteranceYearTitle = c.ForEnteranceYear?.EnteranceYear?.ToString() ?? string.Empty,
            ClassSchedule = c.DateTime ?? string.Empty,
            LessonTypeId = c.LessonTypeId ?? 0,
            LessonTypeTitle = c.LessonType?.LessonTypeTitle ?? string.Empty,
            Credit = c.Credit

        };

        return ServiceResult<ClassDetailDto>.Ok(dto);

    }

    public async Task<ServiceResult<object>> UpdateClassAsync(int currentEmployeeId, int classId, UpdateClassDto dto)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var existingClass = await _uow.Classes.GetByIdAsync(classId)
            ?? throw new NotFoundException("کلاس پیدا نشد.");

        if (existingClass.MajorId != majorId)
            throw new ForbiddenException();

        var lesson = await ResolveLesson(dto.LessonId , dto.LessonTitle);

        var teacher = await _uow.Employees.Query()
            .FirstOrDefaultAsync(e => e.EmployeeId == dto.EmployeeID && e.RoleId == 2)
            ?? throw new NotFoundException("استاد انتخاب‌شده معتبر نیست.");

        if (teacher.MajorId != majorId)
            return ServiceResult<object>.Fail("استاد انتخاب‌شده مربوط به رشته‌ی شما نیست.");

        bool lessonTypeExists = await _uow.LessonTypes.Query().AnyAsync(lt => lt.LessonTypeId == dto.LessonTypeId);
        if (!lessonTypeExists)
            return ServiceResult<object>.Fail("نوع درس انتخاب‌شده معتبر نیست.");

        existingClass.ClassCode = dto.ClassCode;
        existingClass.LessonId = lesson.LessonId;
        existingClass.EmployeeId = dto.EmployeeID;
        existingClass.Capacity = dto.Capacity;
        existingClass.ClassLocationId = dto.ClassLocationId;
        existingClass.FinalExamDate = dto.FinalExamDate;
        existingClass.TermId = dto.TermId;
        existingClass.ForEnteranceYearId = dto.ForEnteranceYearID;
        existingClass.DateTime = dto.ClassSchedule;
        existingClass.LessonTypeId = dto.LessonTypeId;
        existingClass.Credit = dto.Credit;

        _uow.Classes.Update(existingClass);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "کلاس با موفقیت ویرایش شد." });

    }

    public async Task<ServiceResult<object>> DeleteClassAsync(int currentEmployeeId, int classId)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var existingClass = await _uow.Classes.GetByIdAsync(classId)
            ?? throw new NotFoundException("کلاس پیدا نشد.");

        if (existingClass.MajorId != majorId)
            throw new ForbiddenException();

        bool hasRegistrations = await _uow.CourseRegs.Query().AnyAsync(cr => cr.ClassId == classId);
        if (hasRegistrations)
            throw new ConflictException("این کلاس دارای دانشجویان ثبت‌نام‌شده است و قابل حذف نیست.");

        _uow.Classes.Remove(existingClass);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "کلاس با موفقیت حذف شد." });

    }


}
