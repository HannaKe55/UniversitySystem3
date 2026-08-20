using UniversitySystem3.Models;

namespace UniversitySystem3.Repositories;

public interface IUnitOfWork
{
    IRepository<Student> Students { get; }
    IRepository<Employee> Employees { get; }
    IRepository<Login> Login { get; }
    IRepository<Class> Classes { get; }
    IRepository<Lesson> Lessons { get; }
    IRepository<Term> Terms { get; }
    IRepository<CourseReg> CourseRegs { get; }
    IRepository<Status> Statuses { get; }
    IRepository<Major> Majors { get; }
    IRepository<ClassLocation> ClassLocations { get; }
    IRepository<LessonType> LessonTypes { get; }

    Task<int> SaveChangesAsync();
}
