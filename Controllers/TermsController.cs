using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem3.Services.Class;

namespace UniversitySystem3.Controllers;

[ApiController]
[Route("api/terms")]
[Authorize(Roles = "Employee")]
public class TermsController : ControllerBase
{
    private readonly IClassService _service;

    public TermsController(IClassService service)
    {
        _service = service;
    }

    // GET /api/terms/entrance-years
    [HttpGet("entrance-years")]
    public async Task<IActionResult> GetEntranceYears()
    {
        var result = await _service.GetEntranceYearsAsync();
        return Ok(result.Data);
    }
}
