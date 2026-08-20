using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class Term
{
    public int TermId { get; set; }

    public string TermCode { get; set; } = null!;

    public string TermTitle { get; set; } = null!;

    public string OddOreven { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Status { get; set; } = null!;

    public int? EnteranceYear { get; set; }

    public virtual ICollection<Class> ClassForEnteranceYears { get; set; } = new List<Class>();

    public virtual ICollection<Class> ClassTerms { get; set; } = new List<Class>();

    public virtual ICollection<CourseReg> CourseRegs { get; set; } = new List<CourseReg>();

    public virtual ICollection<Student> StudentCurrentTerms { get; set; } = new List<Student>();

    public virtual ICollection<Student> StudentEnteranceYears { get; set; } = new List<Student>();
}
