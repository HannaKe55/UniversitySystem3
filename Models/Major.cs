using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class Major
{
    public int MajorId { get; set; }

    public string? MajorName { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
