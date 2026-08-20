using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Students;

public interface IStudentCourseSelectionServicecs
{
    Task<ServiceResult<object>> GetAvailableClassesAsync(int studentId, string? search,
        List<int>? teacherIds, List<int>? lessonTypeIds, List<int>? lessonIds, List<int>? credits);

    Task<ServiceResult<object>> GetFilterTeachersAsync(int studentId);
    Task<ServiceResult<object>> GetFilterLessonTypesAsync(int studentId);
    Task<ServiceResult<object>> GetFilterLessonsAsync(int studentId);
    Task<ServiceResult<object>> GetFilterCreditsAsync(int studentId);

    Task<ServiceResult<object>> RegisterCoursesAsync(int studentId, RegisterCourseDto dto);
}
