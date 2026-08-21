using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Common.Exceptions;
using UniversitySystem3.Dtos;
using UniversitySystem3.Models;
using UniversitySystem3.Repositories;
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

        var term = await _uow.Terms.Query()
            .FirstOrDefaultAsync(t => t.TermTitle == dto.TermTitle)
            ?? throw new NotFoundException("ترمی با این عنوان پیدا نشد.");

        bool codeExists = await _uow.Students.Query().AnyAsync(s => s.StudentCode == dto.StudentCode);
        if (codeExists)
            throw new ConflictException("این کد دانشجویی قبلاً ثبت شده است.");

        var login = new Login
        {
            Uername = dto.StudentCode,
            Pass = PasswordHasher.Hash(dto.NationalCode)
        };

        var student = new Student
        {
            StudentCode = dto.StudentCode,
            NationalCode = dto.NationalCode,
            FullName = dto.FullName,
            EnteranceYearId = term.TermId,
            MajorId = majorId,
            Login = login
        };

        await _uow.Students.AddAsync(student);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new
        {
            message = "دانشجو با موفقیت ثبت شد.",
            studentId = student.StudentId,
            username = login.Uername,
            password = dto.NationalCode,
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
                    FullName = s.FullName ?? "",
                    StudentCode = s.StudentCode ?? "",
                    MajorName = s.Major?.MajorName ?? ""
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

        var loginId = student.LoginId;

        _uow.Students.Remove(student);
        await _uow.SaveChangesAsync();

        if (loginId.HasValue)
        {
            var login = await _uow.Login.GetByIdAsync(loginId.Value);
            if (login != null)
            {
                _uow.Login.Remove(login);
                await _uow.SaveChangesAsync();
            }
        }

        return ServiceResult<object>.Ok(new { message = "دانشجو با موفقیت حذف شد." });
    }

    // ============ GET /api/students/me ============
    public async Task<ServiceResult<object>> GetMyProfileAsync(int studentId)
    {
        var student = await _uow.Students.Query()
            .Include(s => s.Major)
            .Include(s => s.CurrentTerm)
            .Include(s => s.EnteranceYear)
            .FirstOrDefaultAsync(s => s.StudentId == studentId)
            ?? throw new NotFoundException("اطلاعات دانشجو پیدا نشد.");

        return ServiceResult<object>.Ok(new
        {
            student.StudentId,
            student.StudentCode,
            student.FullName,
            MajorName = student.Major?.MajorName,
            student.MajorId,
            CurrentTerm = student.CurrentTerm?.TermCode,
            EnteranceYear = student.EnteranceYear?.EnteranceYear
        });
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