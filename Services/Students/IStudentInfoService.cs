using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Students;

public interface IStudentInfoService
{
    Task<ServiceResult<StudentSummaryDto>> GetSummaryAsync(int studentId);

    Task<ServiceResult<TermDetailDto>> GetTermDetailAsync(int studentId, int termId);
}
