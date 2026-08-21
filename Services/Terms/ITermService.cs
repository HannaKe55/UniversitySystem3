using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Terms;

public interface ITermService
{
    Task<ServiceResult<object>> GetAllAsync();
    Task<ServiceResult<object>> GetByIdAsync(int id);
    Task<ServiceResult<object>> CreateAsync(int currentEmployeeId, CreateTermDto dto);
    Task<ServiceResult<object>> UpdateAsync(int currentEmployeeId, int id, UpdateTermDto dto);
    Task<ServiceResult<object>> DeleteAsync(int currentEmployeeId, int id);
    Task<ServiceResult<object>> GetEntranceYearsAsync();
}
