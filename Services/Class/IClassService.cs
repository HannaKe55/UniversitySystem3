using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;


namespace UniversitySystem3.Services.Class;

public interface IClassService
{
    // Data for drop-downs
    Task<ServiceResult<object>> GetLessonsAsync();
    Task<ServiceResult<object>> GetLocationsAsync();
    Task<ServiceResult<object>> GetEntranceYearsAsync();
    Task<ServiceResult<object>> GetLessonTypesAsync();

    // main operation on "Class"
    Task<ServiceResult<object>> CreateClassAsync(int currentEmployeeId, CreateClassDto dto);
    Task<ServiceResult<object>> GetClassListAsync(int currentEmployeeId);
    Task<ServiceResult<ClassDetailDto>> GetClassByIdAsync(int currentEmployeeId, int classId);
    Task<ServiceResult<object>> UpdateClassAsync(int currentEmployeeId, int classId, UpdateClassDto dto);
    Task<ServiceResult<object>> DeleteClassAsync(int currentEmployeeId, int classId);

}
