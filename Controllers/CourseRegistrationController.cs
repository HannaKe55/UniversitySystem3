using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;
using UniversitySystem3.Services.Teachers;
using UniversitySystem3.Services.Students;

namespace UniversitySystem3.Controllers;

[ApiController]
[Route("api/course-registrations")]
public class CourseRegistrationsController : ControllerBase
{
    private readonly IStudentCourseSelectionServicecs _studentService;
    private readonly IStudentCourseManagementService _managementService;
    private readonly ITeacherClassService _teacherService;

    public CourseRegistrationsController(
        IStudentCourseSelectionServicecs studentService,
        IStudentCourseManagementService managementService,
        ITeacherClassService teacherService)
    {
        _studentService = studentService;
        _managementService = managementService;
        _teacherService = teacherService;
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    private IActionResult MapError<T>(ServiceResult<T> result)
    {
        return result.ResultType switch
        {
            ServiceResultType.NotFound => NotFound(result.ErrorMessage),
            ServiceResultType.Forbidden => Forbid(),
            _ => BadRequest(result.ErrorMessage)
        };
    }

    // POST /api/course-registrations   (دانشجو ثبت‌نام می‌کنه)
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Register(RegisterCourseDto dto)
    {
        var result = await _studentService.RegisterCoursesAsync(GetCurrentUserId(), dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // DELETE /api/course-registrations/{id}   (کارمند آموزش حذف می‌کنه)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> DeleteByEmployee(int id)
    {
        var result = await _managementService.DeleteCourseRegAsync(GetCurrentUserId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // PATCH /api/course-registrations/{id}/score   (استاد نمره ثبت می‌کنه)
    [HttpPatch("{id}/score")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> SetScore(int id, SetScoreDto dto)
    {
        var result = await _teacherService.SetScoreAsync(GetCurrentUserId(), id, dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // PATCH /api/course-registrations/{id}/remove   (استاد دانشجو رو از کلاس حذف می‌کنه)
    [HttpPatch("{id}/remove")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> RemoveByTeacher(int id)
    {
        var result = await _teacherService.RemoveStudentAsync(GetCurrentUserId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }
}
