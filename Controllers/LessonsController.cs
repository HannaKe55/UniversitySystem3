using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem3.Services.Class;

namespace UniversitySystem3.Controllers;

[ApiController]
[Route("api/lessons")]
[Authorize(Roles = "Employee")]

public class LessonsController : ControllerBase
{
    private readonly IClassService _service;   
    public LessonsController(IClassService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetLessonsAsync();
        return Ok(result.Data);
    }


}
