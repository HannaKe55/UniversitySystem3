using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Common.Exceptions;
using UniversitySystem3.Dtos;
using UniversitySystem3.Repositories;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Teachers;

public class TeachersService : ITeachersService
{

    private readonly IUnitOfWork _uow;

    public TeachersService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ServiceResult<object>> CreateAsync(int currentEmployeeId, RegisterTeacherDto dto)
    {
        var currentEmployee = await _uow.Employees.GetByIdAsync(currentEmployeeId)
                ?? throw new NotFoundException("اطلاعات کارمند لاگین‌شده پیدا نشد.");

        if (currentEmployee.RoleId == 2)
            throw new ForbiddenException("استاد اجازه‌ی ثبت استاد جدید را ندارد.");

        if (currentEmployee.MajorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        bool codeExists = await _uow.Employees.Query().AnyAsync(e => e.EmpCode == dto.EmpCode);
        if (codeExists)
            throw new ConflictException("این کد پرسنلی قبلاً ثبت شده است.");

        bool nationalCodeExists = await _uow.Employees.Query().AnyAsync(e => e.NationalCode == dto.NationalCode);
        if (nationalCodeExists)
            throw new ConflictException("این کد ملی قبلاً ثبت شده است.");

        var teacher = new Models.Employee
        {
            EmpCode = dto.EmpCode,
            NationalCode = dto.NationalCode,
            FullName = dto.FullName,
            Title = dto.Title,
            LastDegree = dto.LastDegree,
            MajorId = currentEmployee.MajorId,
            RoleId = 2
        };

        await _uow.Employees.AddAsync(teacher);
        await _uow.SaveChangesAsync();

        var login = new Models.Login
        {
            Uername = dto.EmpCode,
            Pass = dto.NationalCode,
            EmpId = teacher.EmployeeId
        };

        await _uow.Login.AddAsync(login);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new
        {
            message = "استاد با موفقیت ثبت شد.",
            employeeId = teacher.EmployeeId,
            username = login.Uername,
            password = login.Pass
        });
    }


    public async Task<ServiceResult<object>> GetAllAsync(int currentEmployeeId)
    {
        var currentEmployee = await _uow.Employees.GetByIdAsync(currentEmployeeId)
              ?? throw new NotFoundException("اطلاعات کارمند لاگین‌شده پیدا نشد.");

        if (currentEmployee.MajorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var teachers = await _uow.Employees.Query()
            .Where(e => e.RoleId == 2 && e.MajorId == currentEmployee.MajorId)
            .Select(e => new { e.EmployeeId, e.EmpCode, e.FullName, e.Title, e.LastDegree })
            .ToListAsync();

        return ServiceResult<object>.Ok(teachers);
    }

    public async Task<ServiceResult<object>> GetByIdAsync(int currentEmployeeId, int teacherId)
    {
        var currentEmployee = await _uow.Employees.GetByIdAsync(currentEmployeeId)
              ?? throw new NotFoundException("اطلاعات کارمند لاگین‌شده پیدا نشد.");

        var teacher = await _uow.Employees.Query()
            .Include(e => e.Major)
            .FirstOrDefaultAsync(e => e.EmployeeId == teacherId && e.RoleId == 2)
            ?? throw new NotFoundException("استاد پیدا نشد.");

        if (teacher.MajorId != currentEmployee.MajorId)
            throw new ForbiddenException();

        return ServiceResult<object>.Ok(new
        {
            teacher.EmployeeId,
            teacher.EmpCode,
            teacher.FullName,
            teacher.Title,
            teacher.LastDegree,
            MajorName = teacher.Major?.MajorName
        });

    }

    public async Task<ServiceResult<object>> DeleteAsync(int currentEmployeeId, int teacherId)
    {
        var currentEmployee = await _uow.Employees.GetByIdAsync(currentEmployeeId) ??
             throw new NotFoundException("اطلاعات کارمند لاگین شده پیدا نشد");

        if (currentEmployee.RoleId == 2)
            throw new ForbiddenException("استاد اجازه حذف استاد دیگر را ندارد.");

        var teacher =await _uow.Employees.Query()
            .FirstOrDefaultAsync(e => e.EmployeeId == teacherId && e.RoleId == 2) ??
            throw new NotFoundException("استاد پیدا نشد");

        if(teacher.MajorId != currentEmployee.MajorId)
            throw new ForbiddenException();

        var login = await _uow.Login.Query().FirstOrDefaultAsync(e => e.EmpId == teacherId);
        if(login!= null)
            _uow.Login.Remove(login);

        _uow.Employees.Remove(teacher);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "استاد با موفقیت حذف شد." });
    }
}
