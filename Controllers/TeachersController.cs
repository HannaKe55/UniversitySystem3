using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;
using UniversitySystem3.Services.Teachers;

namespace UniversitySystem3.Controllers;

[ApiController]
[Route("api/teachers")]
[Authorize(Roles ="Employee")]
public class TeachersController : ControllerBase
{
    private readonly ITeachersService _service;
    private readonly ITeacherClassService _teacherClassService;

    public TeachersController(ITeachersService service, ITeacherClassService teacherClassService)
    {
        _service = service;
        _teacherClassService = teacherClassService;
    }

    private int GetCurrentEmployeeId()
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

    [HttpPost]

    public async Task<IActionResult> Create(RegisterTeacherDto dto)
    {
        var result = await _service.CreateAsync(GetCurrentEmployeeId(), dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(GetCurrentEmployeeId());
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(GetCurrentEmployeeId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetCurrentEmployeeId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    [HttpGet("me/classes")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyClasses(
    [FromQuery] List<int>? termIds,
    [FromQuery] List<int>? enteranceYearIds,
    [FromQuery] List<int>? lessonIds )
    {
        var result = await _teacherClassService.GetMyClassesAsync(GetCurrentEmployeeId(),
            termIds, enteranceYearIds, lessonIds);

        return result.Success ? Ok(result.Data) : MapError(result);
    }

    [HttpGet("me/classes/filters/terms")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetRecentTerms()
    {
        var result = await _teacherClassService.GetRecentTermsAsync(GetCurrentEmployeeId());
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // PUT /api/teachers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTeacherDto dto)
    {
        var result = await _service.UpdateAsync(GetCurrentEmployeeId(), id, dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }


}
