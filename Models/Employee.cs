using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? EmpCode { get; set; }

    public string? FullName { get; set; }

    public string? Title { get; set; }

    public string? LastDegree { get; set; }

    public int? MajorId { get; set; }

    public int? RoleId { get; set; }

    public string NationalCode { get; set; } = null!;

    public int? LoginId { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<CourseReg> CourseRegs { get; set; } = new List<CourseReg>();

    public virtual Login? Login { get; set; }

    public virtual Major? Major { get; set; }

    public virtual ICollection<Paticipation> Paticipations { get; set; } = new List<Paticipation>();

    public virtual Role? Role { get; set; }
}
