using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string? StatusTitle { get; set; }

    public virtual ICollection<CourseReg> CourseRegs { get; set; } = new List<CourseReg>();
}
