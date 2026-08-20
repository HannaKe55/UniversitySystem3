using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;
namespace UniversitySystem3.Services.Teachers;

public interface ITeacherClassService
{
    Task<ServiceResult<object>> GetMyClassesAsync(int teacherId,
    List<int>? termIds, List<int>? enteranceYearIds, List<int> lessonIds);

    Task<ServiceResult<object>> GetRecentTermsAsync(int teacherId);
    Task<ServiceResult<object>> GetFilterEnteranceYearsAsync(int teacherId);
    Task<ServiceResult<object>> GetFilterLessonsAsync(int teacherId);

    Task<ServiceResult<ClassStudentListDto>> GetClassStudentsAsync(int teacherId, int classId);
    Task<ServiceResult<object>> SetScoreAsync(int teacherId, int courseRegId, SetScoreDto dto);
    Task<ServiceResult<object>> RemoveStudentAsync(int teacherId, int courseRegId);
}
