using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class Login
{
    public int LoginId { get; set; }

    public string? Uername { get; set; }

    public string? Pass { get; set; }

    public int? EmpId { get; set; }

    public int? StudentId { get; set; }

    public virtual Employee? Emp { get; set; }

    public virtual Student? Student { get; set; }
}
