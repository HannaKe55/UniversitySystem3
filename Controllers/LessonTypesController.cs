using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem3.Services.Class;

namespace UniversitySystem3.Controllers;

[ApiController]
[Route("api/lesson-types")]
[Authorize(Roles = "Employee")]
public class LessonTypesController : ControllerBase
{
    private readonly IClassService _service;

    public LessonTypesController(IClassService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetLessonTypesAsync();
        return Ok(result.Data);
    }

}
