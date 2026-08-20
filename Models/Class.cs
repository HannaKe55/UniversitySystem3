using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public int? LessonId { get; set; }

    public int? TermId { get; set; }

    public int? EmployeeId { get; set; }

    public int? MajorId { get; set; }

    public string? DateTime { get; set; }

    public int? ClassLocationId { get; set; }

    public DateTime? FinalExamDate { get; set; }

    public int? Capacity { get; set; }

    public string ClassCode { get; set; } = null!;

    public int? ForEnteranceYearId { get; set; }

    public int? LessonTypeId { get; set; }

    public int Credit { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ClassLocation? ClassLocation { get; set; }

    public virtual ICollection<CourseReg> CourseRegs { get; set; } = new List<CourseReg>();

    public virtual Employee? Employee { get; set; }

    public virtual Term? ForEnteranceYear { get; set; }

    public virtual Lesson? Lesson { get; set; }

    public virtual LessonType? LessonType { get; set; }

    public virtual Major? Major { get; set; }

    public virtual Term? Term { get; set; }
}
