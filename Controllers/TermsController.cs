using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Class;
using UniversitySystem3.Services.Common;
using UniversitySystem3.Services.Terms;

namespace UniversitySystem3.Controllers;

[ApiController]
[Route("api/terms")]
[Authorize(Roles = "Employee")]
public class TermsController : ControllerBase
{
    private readonly ITermService _service;

    public TermsController(ITermService service)
    {
        _service = service;
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

    // GET /api/terms
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/terms/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // POST /api/terms
    [HttpPost]
    public async Task<IActionResult> Create(CreateTermDto dto)
    {
        var result = await _service.CreateAsync(GetCurrentEmployeeId(), dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // PUT /api/terms/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTermDto dto)
    {
        var result = await _service.UpdateAsync(GetCurrentEmployeeId(), id, dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // DELETE /api/terms/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetCurrentEmployeeId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    // GET /api/terms/entrance-years
    [HttpGet("entrance-years")]
    public async Task<IActionResult> GetEntranceYears()
    {
        var result = await _service.GetEntranceYearsAsync();
        return result.Success ? Ok(result.Data) : MapError(result);
    }
}
