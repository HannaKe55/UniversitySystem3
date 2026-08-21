using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string? StudentCode { get; set; }

    public string? NationalCode { get; set; }

    public string? FullName { get; set; }

    public int? MajorId { get; set; }

    public int? EnteranceYearId { get; set; }

    public int? CurrentTermId { get; set; }

    public int? LoginId { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<CourseReg> CourseRegs { get; set; } = new List<CourseReg>();

    public virtual Term? CurrentTerm { get; set; }

    public virtual Term? EnteranceYear { get; set; }

    public virtual Login? Login { get; set; }

    public virtual Major? Major { get; set; }

    public virtual ICollection<Paticipation> Paticipations { get; set; } = new List<Paticipation>();
}
