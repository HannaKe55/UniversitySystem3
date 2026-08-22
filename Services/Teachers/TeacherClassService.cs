using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Models;
using UniversitySystem3.Dtos;
using UniversitySystem3.Repositories;
using UniversitySystem3.Services.Common;
namespace UniversitySystem3.Services.Teachers;

public class TeacherClassService : ITeacherClassService
{
    
        private readonly IUnitOfWork _uow;

        public TeacherClassService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        private async Task<bool> IsTeacher(int employeeId)
        {
            var employee = await _uow.Employees.GetByIdAsync(employeeId);

            return employee != null && employee.RoleId == 2;

        }

       //استاد میتونه در پنل کاربری خودش کلاس هایی که براش تعریف شده رو ببینه

        public async Task<ServiceResult<object>> GetMyClassesAsync(int teacherId,
            List<int>? termIds, List<int>? enteranceYearIds, List<int> lessonIds)

        {
            if (!await IsTeacher(teacherId))
                return ServiceResult<object>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

            var query = _uow.Classes.Query()
            .Include(c => c.Lesson)
            .Include(c => c.Major)
            .Include(c => c.ForEnteranceYear)
            .Include(c => c.Term)
            .Where(c => c.EmployeeId == teacherId)
            .AsQueryable();


            if ((termIds == null || termIds.Count == 0) &&
                (enteranceYearIds == null || enteranceYearIds.Count == 0) &&
                (lessonIds == null || lessonIds.Count == 0))
            {
                var activeTerm = await _uow.Terms.Query().FirstOrDefaultAsync(e => e.Status == "فعال");
                if (activeTerm != null)
                {
                    query = query.Where(e => e.TermId == activeTerm.TermId);
                }
            }
            else
            {
                if (termIds != null || termIds.Count > 0)
                    query = query.Where(e => termIds.Contains(e.TermId ?? 0));

                if (enteranceYearIds != null || enteranceYearIds.Count > 0)
                    query = query.Where(e => enteranceYearIds.
                    Contains(e.ForEnteranceYearId ?? 0));

                if (lessonIds != null || lessonIds.Count > 0)
                    query = query.Where(e => lessonIds.Contains(e.LessonId ?? 0));

            }

            var classes = await query.ToListAsync();
            var result = classes.Select(c => new TeacherClassDto
            {
                ClassId = c.ClassId,
                ClassCode = c.ClassCode,
                LessonTitle = c.Lesson?.LessonTitle ?? string.Empty,
                MajorName = c.Major?.MajorName ?? string.Empty,
                EnteranceYearTitle = c.ForEnteranceYear?.EnteranceYear?.ToString() ?? string.Empty,
                TermCode = c.Term?.TermCode ?? string.Empty
            }).ToList();

            return ServiceResult<object>.Ok(result);
        }

       //استاد میتونه برای دانشجویان نمره ثبت کنه
        public async Task<ServiceResult<object>> SetScoreAsync(int teacherId, int courseRegId, SetScoreDto dto)
        {
            if (!await IsTeacher(teacherId))
                return ServiceResult<object>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

            var courseReg = await _uow.CourseRegs.Query()
             .Include(cr => cr.Class)
             .FirstOrDefaultAsync(cr => cr.CourseRegId == courseRegId);

            if (courseReg == null)
                return ServiceResult<object>.Fail("این ثبت‌نام درس پیدا نشد.", ServiceResultType.NotFound);

            if (courseReg.Class?.EmployeeId != teacherId)
                return ServiceResult<object>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

            courseReg.Score = dto.Score;

            string statusTitle = dto.Score >= 10 ? "قبول شده" : "رد شده";
            var status = await _uow.Statuses.Query().FirstOrDefaultAsync(s => s.StatusTitle == statusTitle);

            if (status == null)
                return ServiceResult<object>.Fail($"وضعیت '{statusTitle}' در سیستم تعریف نشده است.");

            courseReg.StatusId = status.StatusId;

            _uow.CourseRegs.Update(courseReg);
            await _uow.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "نمره با موفقیت ثبت شد." });
        }
        //استاد میتونه دانشجو رو از کلاس مربوطه حذف کنه
        public async Task<ServiceResult<object>> RemoveStudentAsync(int teacherId, int courseRegId)
        {
            if (!await IsTeacher(teacherId))
                return ServiceResult<object>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

            var courseReg = await _uow.CourseRegs.Query()
                .Include(cr => cr.Class)
                .FirstOrDefaultAsync(cr => cr.CourseRegId == courseRegId);

            if (courseReg == null)
                return ServiceResult<object>.Fail("این ثبت‌نام درس پیدا نشد.", ServiceResultType.NotFound);

            if (courseReg.Class?.EmployeeId != teacherId)
                return ServiceResult<object>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

            var removedStatus = await _uow.Statuses.Query()
                .FirstOrDefaultAsync(s => s.StatusTitle == "حذف شده");

            if (removedStatus == null)
                return ServiceResult<object>.Fail("وضعیت 'حذف شده' در سیستم تعریف نشده است.");

            courseReg.Score = 0;
            courseReg.StatusId = removedStatus.StatusId;

            _uow.CourseRegs.Update(courseReg);
            await _uow.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "دانشجو با موفقیت از کلاس حذف شد." });



        }
    //استاد میتونه کلاس های خودش رو بر اساس اسم درس فیلتر کنه
        public async Task<ServiceResult<object>> GetFilterLessonsAsync(int teacherId)
        {
            if (!await IsTeacher(teacherId))
                return ServiceResult<object>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

            var lessons = await _uow.Classes.Query()
                .Where(c => c.EmployeeId == teacherId)
                .Include(c => c.Lesson)
                .Select(c => c.Lesson)
                .Where(l => l != null)
                .Distinct()
                .Select(l => new { l!.LessonId, l.LessonTitle })
                .ToListAsync();

            return ServiceResult<object>.Ok(lessons);
        }
    //استاد میتونه کلاس های خودش رو بر اساس سال ورودی فیلتر کنه
        public async Task<ServiceResult<object>> GetFilterEnteranceYearsAsync(int teacherId)
        {
            if (!await IsTeacher(teacherId))
                return ServiceResult<object>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

            var years = await _uow.Classes.Query()
                .Where(c => c.EmployeeId == teacherId)
                .Include(c => c.ForEnteranceYear)
                .Select(c => c.ForEnteranceYear)
                .Where(t => t != null)
                .Distinct()
                .Select(t => new { t!.TermId, t.EnteranceYear })
                .ToListAsync();

            return ServiceResult<object>.Ok(years);
        }

        public async Task<ServiceResult<ClassStudentListDto>> GetClassStudentsAsync(int teacherId, int classId)
        {
            if (!await IsTeacher(teacherId))
                return ServiceResult<ClassStudentListDto>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

            var targetClass = await _uow.Classes.Query()
                .Include(c => c.Lesson)
                .Include(c => c.Term)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (targetClass == null)
                return ServiceResult<ClassStudentListDto>.Fail("کلاس پیدا نشد.", ServiceResultType.NotFound);

            if (targetClass.EmployeeId != teacherId)
                return ServiceResult<ClassStudentListDto>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

            var students = await _uow.CourseRegs.Query()
                .Include(cr => cr.Student)
                    .ThenInclude(s => s!.Major)
                .Include(cr => cr.Status)
                .Where(cr => cr.ClassId == classId
                    && (cr.Status == null || cr.Status.StatusTitle != "حذف شده"))
                .Select(cr => new ClassStudentDto
                {
                    CourseRegId = cr.CourseRegId,
                    StudentId = cr.StudentId ?? 0,
                    FullName = cr.Student!.FullName ?? string.Empty,
                    StudentCode = cr.Student.StudentCode ?? string.Empty,
                    MajorName = cr.Student.Major!.MajorName ?? string.Empty,
                })
                .ToListAsync();

            var result = new ClassStudentListDto
            {
                LessonTitle = targetClass.Lesson?.LessonTitle ?? string.Empty,
                TermCode = targetClass.Term?.TermCode ?? string.Empty,
                Students = students
            };

            return ServiceResult<ClassStudentListDto>.Ok(result);
        }

    // استاد میاد 8 ترم اخیرش رو میبینه که بعد بره توی هر ترم کلاس هاشو ببینه
        public async Task<ServiceResult<object>> GetRecentTermsAsync(int teacherId)
        {
            if (!await IsTeacher(teacherId))
                return ServiceResult<object>.Fail("دسترسی غیرمجاز.", ServiceResultType.Forbidden);

            var terms = await _uow.Classes.Query()
                .Where(c => c.EmployeeId == teacherId)
                .Include(c => c.Term)
                .Select(c => c.Term)
                .Where(t => t != null)
                .Distinct()
                .OrderByDescending(t => t!.StartDate)
                .Take(8)
                .Select(t => new { t!.TermId, t.TermCode })
                .ToListAsync();

            return ServiceResult<object>.Ok(terms);
        }


}

