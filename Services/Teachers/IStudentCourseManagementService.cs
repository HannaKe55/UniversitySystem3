using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Teachers;

public interface IStudentCourseManagementService
{
    Task<ServiceResult<object>> GetStudentsAsync
       (int currentEmployeeId, string? search);
    Task<ServiceResult<StudentCourseSelectionDto>> GetStudentCourseSelectionAsync
        (int currentEmployeeId, int studentId);

    Task<ServiceResult<object>> DeleteCourseRegAsync
        (int currentEmployeeId, int courseRegId);
}
