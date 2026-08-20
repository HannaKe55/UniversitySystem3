using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Dtos;
using UniversitySystem3.Common.Exceptions;
using UniversitySystem3.Repositories;
using UniversitySystem3.Models;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Students;



public class StudentsService : IStudentService
{
    private readonly IUnitOfWork _uow;

    public StudentsService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private async Task<int?> GetEmployeeMajorId(int employeeId)
    {
        var employee = await _uow.Employees.GetByIdAsync(employeeId);
        return employee?.MajorId;
    }

    // ============ POST /api/students ============
    public async Task<ServiceResult<object>> CreateAsync(int currentEmployeeId, RegisterStudentDto dto)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var term = await _uow.Terms.Query().FirstOrDefaultAsync(t => t.TermTitle == dto.TermTitle)
            ?? throw new NotFoundException("ترمی با این عنوان پیدا نشد.");

        bool codeExists = await _uow.Students.Query().AnyAsync(s => s.StudentCode == dto.StudentCode);
        if (codeExists)
            throw new ConflictException("این کد دانشجویی قبلاً ثبت شده است.");

        var student = new Student
        {
            StudentCode = dto.StudentCode,
            NationalCode = dto.NationalCode,
            FullName = dto.FullName,
            EnteranceYearId = term.TermId,
            MajorId = majorId
        };

        await _uow.Students.AddAsync(student);
        await _uow.SaveChangesAsync();

        var login = new Login
        {
            Uername = dto.StudentCode,
            Pass = dto.NationalCode,
            StudentId = student.StudentId
        };

        await _uow.Login.AddAsync(login);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new
        {
            message = "دانشجو با موفقیت ثبت شد.",
            studentId = student.StudentId,
            username = login.Uername,
            password = login.Pass,
            term = term.TermTitle
        });
    }

    // ============ GET /api/students ============
    public async Task<ServiceResult<object>> GetAllAsync(int currentEmployeeId, string? search)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var query = _uow.Students.Query()
            .Include(s => s.Major)
            .Include(s => s.EnteranceYear)
            .Where(s => s.MajorId == majorId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                (s.FullName != null && s.FullName.Contains(search)) ||
                (s.StudentCode != null && s.StudentCode.Contains(search)));
        }

        var students = await query.ToListAsync();

        var grouped = students
            .GroupBy(s => s.EnteranceYear?.EnteranceYear)
            .OrderBy(g => g.Key)
            .Select(g => new StudentYearGroupDto
            {
                EnteranceYear = g.Key?.ToString() ?? "نامشخص",
                Students = g.Select(s => new StudentListItemDto
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName ?? string.Empty,
                    StudentCode = s.StudentCode ?? string.Empty,
                    MajorName = s.Major?.MajorName ?? string.Empty
                }).ToList()
            })
            .ToList();

        return ServiceResult<object>.Ok(grouped);
    }

    // ============ GET /api/students/{id} ============
    public async Task<ServiceResult<object>> GetByIdAsync(int currentEmployeeId, int studentId)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var student = await _uow.Students.Query()
            .Include(s => s.Major)
            .Include(s => s.EnteranceYear)
            .Include(s => s.CurrentTerm)
            .FirstOrDefaultAsync(s => s.StudentId == studentId)
            ?? throw new NotFoundException("دانشجو پیدا نشد.");

        if (student.MajorId != majorId)
            throw new ForbiddenException();

        return ServiceResult<object>.Ok(new
        {
            student.StudentId,
            student.StudentCode,
            student.NationalCode,
            student.FullName,
            MajorName = student.Major?.MajorName,
            EnteranceYear = student.EnteranceYear?.EnteranceYear,
            CurrentTerm = student.CurrentTerm?.TermCode
        });
    }

    // ============ DELETE /api/students/{id} ============
    public async Task<ServiceResult<object>> DeleteAsync(int currentEmployeeId, int studentId)
    {
        var majorId = await GetEmployeeMajorId(currentEmployeeId);
        if (majorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var student = await _uow.Students.GetByIdAsync(studentId)
            ?? throw new NotFoundException("دانشجو پیدا نشد.");

        if (student.MajorId != majorId)
            throw new ForbiddenException();

        // اول اکانت لاگین مرتبط رو حذف کن (چون به این دانشجو وابسته‌ست)
        var login = await _uow.Login.Query().FirstOrDefaultAsync(l => l.StudentId == studentId);
        if (login != null)
            _uow.Login.Remove(login);

        _uow.Students.Remove(student);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "دانشجو با موفقیت حذف شد." });
    }

    // ============ GET /api/students/me ============
    public async Task<ServiceResult<object>> GetMyProfileAsync(int studentId)
    {
        var student = await _uow.Students.Query()
            .Include(s => s.Major)
            .Include(s => s.CurrentTerm)
            .FirstOrDefaultAsync(s => s.StudentId == studentId)
            ?? throw new NotFoundException("اطلاعات دانشجو پیدا نشد.");

        return ServiceResult<object>.Ok(student);
    }

    // ============ PATCH /api/students/me ============
    public async Task<ServiceResult<object>> UpdateMyProfileAsync(int studentId, CompleteStudentProfileDto dto)
    {
        var student = await _uow.Students.GetByIdAsync(studentId)
            ?? throw new NotFoundException("اطلاعات دانشجو پیدا نشد.");

        student.FullName = dto.FullName;
        student.MajorId = dto.MajorId;

        _uow.Students.Update(student);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "پروفایل با موفقیت تکمیل شد." });
    }
}
