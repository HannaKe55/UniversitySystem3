using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Auth;

public interface IAuthService
{
    Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto dto);

}
