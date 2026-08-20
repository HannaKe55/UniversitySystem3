using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Students;

public interface IStudentService
{
    // Managing by "Karmand Amoozesh"
    Task<ServiceResult<object>> CreateAsync(int currentEmployeeId, RegisterStudentDto dto);
    Task<ServiceResult<object>> GetAllAsync(int currentEmployeeId, string? search);
    Task<ServiceResult<object>> GetByIdAsync(int currentEmployeeId, int studentId);
    Task<ServiceResult<object>> DeleteAsync(int currentEmployeeId, int studentId);

    // The student himself
    Task<ServiceResult<object>> GetMyProfileAsync(int studentId);
    Task<ServiceResult<object>> UpdateMyProfileAsync(int studentId, CompleteStudentProfileDto dto);
}
