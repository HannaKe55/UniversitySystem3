using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Employees;

public interface IEmployeeService
{
    Task<ServiceResult<object>> CreateAsync(int currentEmployeeId, RegisterEmployeeDto dto);
    Task<ServiceResult<object>> GetAllAsync(int currentEmployeeId);
    Task<ServiceResult<object>> GetByIdAsync(int currentEmployeeId, int employeeId);
    Task<ServiceResult<object>> UpdateAsync(int currentEmployeeId, int employeeId, UpdateTeacherDto dto);
    Task<ServiceResult<object>> DeleteAsync(int currentEmployeeId, int employeeId);
}
