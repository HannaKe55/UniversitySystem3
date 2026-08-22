using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversitySystem3.Dtos;
using UniversitySystem3.Services.Common;
using UniversitySystem3.Services.Employees;

namespace UniversitySystem3.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize(Roles = "Employee")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
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

    [HttpPost]
    public async Task<IActionResult> Create(RegisterEmployeeDto dto)
    {
        var result = await _service.CreateAsync(1/*GetCurrentEmployeeId()*/, dto);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTeacherDto dto)
    {
        var result = await _service.UpdateAsync(GetCurrentEmployeeId(), id, dto);
        return result.Success ? Ok(result.Data) : MapError(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetCurrentEmployeeId(), id);
        return result.Success ? Ok(result.Data) : MapError(result);
    }
}