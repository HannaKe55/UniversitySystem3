using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Common.Exceptions;
using UniversitySystem3.Dtos;
using UniversitySystem3.Models;
using UniversitySystem3.Repositories;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Employees;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _uow;

    public EmployeeService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // ============ POST /api/employees ============
    public async Task<ServiceResult<object>> CreateAsync(int currentEmployeeId, RegisterEmployeeDto dto)
    {
        //var currentEmployee = await _uow.Employees.GetByIdAsync(currentEmployeeId)
            //?? throw new NotFoundException("اطلاعات کارمند لاگین‌شده پیدا نشد.");

        //if (currentEmployee.RoleId == 2)
           // throw new ForbiddenException("استاد اجازه‌ی ثبت کارمند آموزش جدید را ندارد.");

        //if (currentEmployee.MajorId == null)
           // return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        //bool codeExists = await _uow.Employees.Query().AnyAsync(e => e.EmpCode == dto.EmpCode);
        //if (codeExists)
            //throw new ConflictException("این کد پرسنلی قبلاً ثبت شده است.");

       // bool nationalCodeExists = await _uow.Employees.Query().AnyAsync(e => e.NationalCode == dto.NationalCode);
        //if (nationalCodeExists)
            //throw new ConflictException("این کد ملی قبلاً ثبت شده است.");

        var login = new Login
        {
            Uername = dto.EmpCode,
            Pass = PasswordHasher.Hash(dto.NationalCode)
        };

        var newEmployee = new Employee
        {
            EmpCode = dto.EmpCode,
            NationalCode = dto.NationalCode,
            FullName = dto.FullName,
            Title = dto.Title,
            LastDegree = dto.LastDegree,
            MajorId = 2 ,//currentEmployee.MajorId,
            RoleId = 1,   // کارمند آموزش
            Login = login
        };

        await _uow.Employees.AddAsync(newEmployee);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new
        {
            message = "کارمند آموزش با موفقیت ثبت شد.",
            employeeId = newEmployee.EmployeeId,
            username = login.Uername,
            password = dto.NationalCode
        });
    }

    // ============ GET /api/employees ============
    public async Task<ServiceResult<object>> GetAllAsync(int currentEmployeeId)
    {
        var currentEmployee = await _uow.Employees.GetByIdAsync(currentEmployeeId)
            ?? throw new NotFoundException("اطلاعات کارمند لاگین‌شده پیدا نشد.");

        if (currentEmployee.MajorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var employees = await _uow.Employees.Query()
            .Where(e => e.RoleId == 1 && e.MajorId == currentEmployee.MajorId)
            .Select(e => new { e.EmployeeId, e.EmpCode, e.FullName, e.Title, e.LastDegree })
            .ToListAsync();

        return ServiceResult<object>.Ok(employees);
    }

    // ============ GET /api/employees/{id} ============
    public async Task<ServiceResult<object>> GetByIdAsync(int currentEmployeeId, int employeeId)
    {
        var currentEmployee = await _uow.Employees.GetByIdAsync(currentEmployeeId)
            ?? throw new NotFoundException("اطلاعات کارمند لاگین‌شده پیدا نشد.");

        var employee = await _uow.Employees.Query()
            .Include(e => e.Major)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.RoleId == 1)
            ?? throw new NotFoundException("کارمند آموزش پیدا نشد.");

        if (employee.MajorId != currentEmployee.MajorId)
            throw new ForbiddenException();

        return ServiceResult<object>.Ok(new
        {
            employee.EmployeeId,
            employee.EmpCode,
            employee.FullName,
            employee.Title,
            employee.LastDegree,
            MajorName = employee.Major?.MajorName
        });
    }

    // ============ PUT /api/employees/{id} ============
    public async Task<ServiceResult<object>> UpdateAsync(int currentEmployeeId, int employeeId, UpdateTeacherDto dto)
    {
        var currentEmployee = await _uow.Employees.GetByIdAsync(currentEmployeeId)
            ?? throw new NotFoundException("اطلاعات کارمند لاگین‌شده پیدا نشد.");

        if (currentEmployee.RoleId == 2)
            throw new ForbiddenException("استاد اجازه‌ی ویرایش اطلاعات کارمند آموزش را ندارد.");

        if (currentEmployee.MajorId == null)
            return ServiceResult<object>.Fail("رشته‌ی این کارمند آموزش مشخص نیست.");

        var employee = await _uow.Employees.Query()
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.RoleId == 1)
            ?? throw new NotFoundException("کارمند آموزش پیدا نشد.");

        if (employee.MajorId != currentEmployee.MajorId)
            throw new ForbiddenException();

        employee.FullName = dto.FullName;
        employee.Title = dto.Title;
        employee.LastDegree = dto.LastDegree;

        _uow.Employees.Update(employee);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "اطلاعات کارمند آموزش با موفقیت ویرایش شد." });
    }

    // ============ DELETE /api/employees/{id} ============
    public async Task<ServiceResult<object>> DeleteAsync(int currentEmployeeId, int employeeId)
    {
        var currentEmployee = await _uow.Employees.GetByIdAsync(currentEmployeeId)
            ?? throw new NotFoundException("اطلاعات کارمند لاگین‌شده پیدا نشد.");

        if (currentEmployee.RoleId == 2)
            throw new ForbiddenException("استاد اجازه‌ی حذف کارمند آموزش را ندارد.");

        if (employeeId == currentEmployeeId)
            throw new ConflictException("امکان حذف حساب کاربری خودتان وجود ندارد.");

        var employee = await _uow.Employees.Query()
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.RoleId == 1)
            ?? throw new NotFoundException("کارمند آموزش پیدا نشد.");

        if (employee.MajorId != currentEmployee.MajorId)
            throw new ForbiddenException();

        var loginId = employee.LoginId;

        _uow.Employees.Remove(employee);
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

        return ServiceResult<object>.Ok(new { message = "کارمند آموزش با موفقیت حذف شد." });
    }
}
