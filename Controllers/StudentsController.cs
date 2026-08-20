using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Teachers;
using UniversitySystem3.Services.Common;
using UniversitySystem3.Services.Students;

namespace UniversitySystem3.Controllers;


[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    private readonly IStudentCourseSelectionServicecs _courseSelectionService;
    private readonly IStudentInfoService _infoService;
    private readonly IStudentCourseManagementService _managementService;

    public StudentsController(
        IStudentService service,
        IStudentCourseSelectionServicecs courseSelectionService,
        IStudentInfoService infoService,
        IStudentCourseManagementService managementService)
    {
        _service = service;
        _courseSelectionService = courseSelectionService;
        _infoService = infoService;
        _managementService = managementService;
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

    // ============ عملیات کارمند آموزش روی دانشجویان ============

    // POST /api/students
    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Create(RegisterStudentDto dto)
    {
        var result = await _service.CreateAsync(GetCurrentUserId(), dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/students
    [HttpGet]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var result = await _service.GetAllAsync(GetCurrentUserId(), search);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/students/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(GetCurrentUserId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // DELETE /api/students/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetCurrentUserId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/students/{id}/course-registrations
    // کارمند آموزش می‌بینه یه دانشجوی خاص، توی ترم فعال چه دروسی انتخاب کرده
    [HttpGet("{id}/course-registrations")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetStudentCourseSelection(int id)
    {
        var result = await _managementService.GetStudentCourseSelectionAsync(GetCurrentUserId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // ============ عملیات خودِ دانشجو روی پروفایل و انتخاب واحد خودش ============

    // GET /api/students/me
    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await _service.GetMyProfileAsync(GetCurrentUserId());
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // PATCH /api/students/me
    [HttpPatch("me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> UpdateMyProfile(CompleteStudentProfileDto dto)
    {
        var result = await _service.UpdateMyProfileAsync(GetCurrentUserId(), dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/students/me/available-classes
    [HttpGet("me/available-classes")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetAvailableClasses(
        [FromQuery] string? search,
        [FromQuery] List<int>? teacherIds,
        [FromQuery] List<int>? lessonTypeIds,
        [FromQuery] List<int>? lessonIds,
        [FromQuery] List<int>? credits)
    {
        var result = await _courseSelectionService.GetAvailableClassesAsync(
            GetCurrentUserId(), search, teacherIds, lessonTypeIds, lessonIds, credits);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/students/me/available-classes/filters/teachers
    [HttpGet("me/available-classes/filters/teachers")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetFilterTeachers()
    {
        var result = await _courseSelectionService.GetFilterTeachersAsync(GetCurrentUserId());
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/students/me/available-classes/filters/lesson-types
    [HttpGet("me/available-classes/filters/lesson-types")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetFilterLessonTypes()
    {
        var result = await _courseSelectionService.GetFilterLessonTypesAsync(GetCurrentUserId());
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/students/me/available-classes/filters/lessons
    [HttpGet("me/available-classes/filters/lessons")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetFilterLessons()
    {
        var result = await _courseSelectionService.GetFilterLessonsAsync(GetCurrentUserId());
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/students/me/available-classes/filters/credits
    [HttpGet("me/available-classes/filters/credits")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetFilterCredits()
    {
        var result = await _courseSelectionService.GetFilterCreditsAsync(GetCurrentUserId());
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/students/me/summary
    [HttpGet("me/summary")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _infoService.GetSummaryAsync(GetCurrentUserId());
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/students/me/terms/{termId}
    [HttpGet("me/terms/{termId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetTermDetail(int termId)
    {
        var result = await _infoService.GetTermDetailAsync(GetCurrentUserId(), termId);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

}
