using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Common.Exceptions;
using UniversitySystem3.Models;
namespace UniversitySystem3.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly UniversityDBContext _context;

    private IRepository<Student>? _students;
    private IRepository<Employee>? _employees;
    private IRepository<Login>? _login;
    private IRepository<Class>? _classes;
    private IRepository<Lesson>? _lessons;
    private IRepository<Term>? _terms;
    private IRepository<CourseReg>? _courseRegs;
    private IRepository<Status>? _statuses;
    private IRepository<Major>? _majors;
    private IRepository<ClassLocation>? _classLocations;
    private IRepository<LessonType>? _lessonTypes;

    public UnitOfWork(UniversityDBContext context)
    {
        _context = context;
    }

    public IRepository<Student> Students => _students ??= new Repository<Student>(_context);
    public IRepository<Employee> Employees => _employees ??= new Repository<Employee>(_context);
    public IRepository<Login> Login => _login ??= new Repository<Login>(_context);
    public IRepository<Class> Classes => _classes ??= new Repository<Class>(_context);
    public IRepository<Lesson> Lessons => _lessons ??= new Repository<Lesson>(_context);
    public IRepository<Term> Terms => _terms ??= new Repository<Term>(_context);
    public IRepository<CourseReg> CourseRegs => _courseRegs ??= new Repository<CourseReg>(_context);
    public IRepository<Status> Statuses => _statuses ??= new Repository<Status>(_context);
    public IRepository<Major> Majors => _majors ??= new Repository<Major>(_context);
    public IRepository<ClassLocation> ClassLocations => _classLocations ??= new Repository<ClassLocation>(_context);
    public IRepository<LessonType> LessonTypes => _lessonTypes ??= new Repository<LessonType>(_context);

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConflictException(
                "این اطلاعات هم‌زمان توسط شخص دیگری تغییر کرده است. لطفاً دوباره تلاش کنید.", ex);
        }
        catch (DbUpdateException ex)
        {
            // معمولاً این خطا مربوط به نقض محدودیت‌های دیتابیسه (Foreign Key، Unique و غیره)
            throw new ConflictException(
                "امکان ذخیره‌سازی اطلاعات وجود ندارد؛ احتمالاً این عملیات با یک محدودیت دیتابیسی (مانند تکراری بودن یا ارجاع نامعتبر) در تعارض است.", ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseWriteException("خطای غیرمنتظره در ذخیره‌سازی اطلاعات.", ex);
        }
    }

}
