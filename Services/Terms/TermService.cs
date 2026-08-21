using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Common.Exceptions;
using UniversitySystem3.Dtos;
using UniversitySystem3.Models;
using UniversitySystem3.Repositories;
using UniversitySystem3.Services.Common;

namespace UniversitySystem3.Services.Terms;

public class TermService : ITermService
{
    private readonly IUnitOfWork _uow;

    public TermService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private async Task EnsureIsEducationEmployee(int employeeId)
    {
        var emp = await _uow.Employees.GetByIdAsync(employeeId)
            ?? throw new NotFoundException("کارمند پیدا نشد.");

        if (emp.RoleId != 1)
            throw new ForbiddenException("فقط کارمند آموزش مجاز به مدیریت ترم‌ها است.");
    }

    public async Task<ServiceResult<object>> GetAllAsync()
    {
        var terms = await _uow.Terms.Query()
            .OrderByDescending(t => t.TermId)
            .Select(t => new TermListItemDto
            {
                TermId = t.TermId,
                TermCode = t.TermCode,
                TermTitle = t.TermTitle,
                OddOreven = t.OddOreven,
                Status = t.Status,
                EnteranceYear = t.EnteranceYear,
                StartDate = t.StartDate,
                EndDate = t.EndDate
            }).ToListAsync();

        return ServiceResult<object>.Ok(terms);
    }

    public async Task<ServiceResult<object>> GetByIdAsync(int id)
    {
        var term = await _uow.Terms.GetByIdAsync(id)
            ?? throw new NotFoundException("ترم پیدا نشد.");

        return ServiceResult<object>.Ok(new TermListItemDto
        {
            TermId = term.TermId,
            TermCode = term.TermCode,
            TermTitle = term.TermTitle,
            OddOreven = term.OddOreven,
            Status = term.Status,
            EnteranceYear = term.EnteranceYear,
            StartDate = term.StartDate,
            EndDate = term.EndDate
        });
    }

    public async Task<ServiceResult<object>> CreateAsync(int currentEmployeeId, CreateTermDto dto)
    {
        await EnsureIsEducationEmployee(currentEmployeeId);

        bool codeExists = await _uow.Terms.Query().AnyAsync(t => t.TermCode == dto.TermCode);
        if (codeExists)
            throw new ConflictException("این کد ترم قبلاً ثبت شده است.");

        var term = new Term
        {
            TermCode = dto.TermCode,
            TermTitle = dto.TermTitle,
            OddOreven = dto.OddOreven,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            EnteranceYear = dto.EnteranceYear
        };

        await _uow.Terms.AddAsync(term);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new
        {
            message = "ترم با موفقیت ثبت شد.",
            termId = term.TermId
        });
    }

    public async Task<ServiceResult<object>> UpdateAsync(int currentEmployeeId, int id, UpdateTermDto dto)
    {
        await EnsureIsEducationEmployee(currentEmployeeId);

        var term = await _uow.Terms.GetByIdAsync(id)
            ?? throw new NotFoundException("ترم پیدا نشد.");

        term.TermTitle = dto.TermTitle;
        term.OddOreven = dto.OddOreven;
        term.StartDate = dto.StartDate;
        term.EndDate = dto.EndDate;
        term.Status = dto.Status;
        term.EnteranceYear = dto.EnteranceYear;

        _uow.Terms.Update(term);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "ترم با موفقیت ویرایش شد." });
    }

    public async Task<ServiceResult<object>> DeleteAsync(int currentEmployeeId, int id)
    {
        await EnsureIsEducationEmployee(currentEmployeeId);

        var term = await _uow.Terms.GetByIdAsync(id)
            ?? throw new NotFoundException("ترم پیدا نشد.");

        // چک وابستگی‌ها
        bool hasClasses = await _uow.Classes.Query().AnyAsync(c => c.TermId == id || c.ForEnteranceYearId == id);
        if (hasClasses)
            throw new ConflictException("این ترم دارای کلاس است و قابل حذف نیست.");

        bool hasStudents = await _uow.Students.Query()
            .AnyAsync(s => s.EnteranceYearId == id || s.CurrentTermId == id);
        if (hasStudents)
            throw new ConflictException("این ترم به دانشجویان مرتبط است و قابل حذف نیست.");

        bool hasRegs = await _uow.CourseRegs.Query().AnyAsync(cr => cr.TermId == id);
        if (hasRegs)
            throw new ConflictException("این ترم دارای ثبت‌نام است و قابل حذف نیست.");

        _uow.Terms.Remove(term);
        await _uow.SaveChangesAsync();

        return ServiceResult<object>.Ok(new { message = "ترم با موفقیت حذف شد." });
    }

    public async Task<ServiceResult<object>> GetEntranceYearsAsync()
    {
        var entranceYears = await _uow.Terms.Query()
            .Where(t => t.EnteranceYear != null)
            .Select(t => new { t.TermId, t.EnteranceYear, t.TermTitle })
            .ToListAsync();

        return ServiceResult<object>.Ok(entranceYears);
    }
}
