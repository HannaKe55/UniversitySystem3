using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Class;
using UniversitySystem3.Services.Common;
using Microsoft.AspNetCore.Authorization;
using UniversitySystem3.Services.Teachers;


namespace UniversitySystem3.Controllers;

[ApiController]
[Route("api/classes")]
[Authorize(Roles ="Employee")]
public class ClassesController : ControllerBase
{
    private readonly IClassService _service;
    private readonly ITeacherClassService _teacherClassService;

    public ClassesController(IClassService service, ITeacherClassService teacherClassService)
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
    public async Task<IActionResult> Create(CreateClassDto dto)
    {
        var result = await _service.CreateClassAsync(GetCurrentEmployeeId(), dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/classes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetClassListAsync(GetCurrentEmployeeId());
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/classes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetClassByIdAsync(GetCurrentEmployeeId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // PUT /api/classes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateClassDto dto)
    {
        var result = await _service.UpdateClassAsync(GetCurrentEmployeeId(), id, dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // DELETE /api/classes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteClassAsync(GetCurrentEmployeeId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    [HttpGet("{classId}/course-registrations")]
    public async Task<IActionResult> GetClassStudents(int classId)
    {
        var result = await _teacherClassService.GetClassStudentsAsync(GetCurrentEmployeeId(), classId);
        return result.Success ? Ok(result.Data) : MapError(result);
    }


}
