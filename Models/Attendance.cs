using System;
using System.Collections.Generic;

namespace UniversitySystem3.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int? StudentId { get; set; }

    public int? ClassId { get; set; }

    public DateOnly? ClassDate { get; set; }

    public string? AttStatus { get; set; }

    public virtual Class? Class { get; set; }

    public virtual Student? Student { get; set; }
}
