using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class CourseReg
{
    public int CourseRegId { get; set; }

    public int? StudentId { get; set; }

    public int? ClassId { get; set; }

    public int? EmployeeId { get; set; }

    public int? TermId { get; set; }

    public double? Score { get; set; }

    public int? StatusId { get; set; }

    public virtual Class? Class { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Status? Status { get; set; }

    public virtual Student? Student { get; set; }

    public virtual Term? Term { get; set; }
}
