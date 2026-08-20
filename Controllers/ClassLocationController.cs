using Microsoft.AspNetCore.Mvc;
using UniversitySystem3.Services.Class;

namespace UniversitySystem3.Controllers;

public class ClassLocationController: ControllerBase
{
    private readonly IClassService _service;

    public ClassLocationController(IClassService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetLocationsAsync();
        return Ok(result.Data);
    }
}
