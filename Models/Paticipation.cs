using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class Paticipation
{
    public int Cpid { get; set; }

    public int? EmpId { get; set; }

    public int? StudentId { get; set; }

    public string? Description { get; set; }

    public virtual Employee? Emp { get; set; }

    public virtual Student? Student { get; set; }
}
