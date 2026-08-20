using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Teachers;

public interface ITeachersService
{
    Task<ServiceResult<object>> CreateAsync(int currentEmployeeId, RegisterTeacherDto dto);
    Task<ServiceResult<object>> GetAllAsync(int currentEmployeeId);
    Task<ServiceResult<object>> GetByIdAsync(int currentEmployeeId, int teacherId);
    Task<ServiceResult<object>> DeleteAsync(int currentEmployeeId, int teacherId);
}
