using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Dtos;
using UniversitySystem3.Repositories;
using UniversitySystem3.Services;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly TokenService _tokenService;

    public AuthService(IUnitOfWork uow, TokenService tokenService)
    {
        _uow = uow;
        _tokenService = tokenService;
    }

    public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var login = await _uow.Login.Query()
            .Include(l => l.Employee).ThenInclude(e => e!.Role)
            .Include(l => l.Student)
            .FirstOrDefaultAsync(l => l.Uername == dto.Username);

        if (login == null || string.IsNullOrEmpty(login.Pass) ||
            !PasswordHasher.Verify(dto.Password, login.Pass))
        {
            return ServiceResult<LoginResponseDto>.Fail("نام کاربری یا رمز عبور اشتباه است.");
        }

        // کاربر کارمند/استاد است
        if (login.Employee != null)
        {
            var roleName = login.Employee.Role?.RoleName ?? "Employee";
            var token = _tokenService.GenerateToken(login.Employee.EmployeeId, roleName, login.Uername!);

            return ServiceResult<LoginResponseDto>.Ok(new LoginResponseDto
            {
                Token = token,
                Role = roleName,
                UserId = login.Employee.EmployeeId,
                FullName = login.Employee.FullName ?? ""
            });
        }

        // کاربر دانشجو است
        if (login.Student != null)
        {
            var token = _tokenService.GenerateToken(login.Student.StudentId, "Student", login.Uername!);

            return ServiceResult<LoginResponseDto>.Ok(new LoginResponseDto
            {
                Token = token,
                Role = "Student",
                UserId = login.Student.StudentId,
                FullName = login.Student.FullName ?? ""
            });
        }

        return ServiceResult<LoginResponseDto>.Fail("حساب کاربری معتبر نیست.");
    }
}