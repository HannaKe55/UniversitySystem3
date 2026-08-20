namespace UniversitySystem3.Dtos;

public class ClassDetailDto
{
    public int ClassId { get; set; }
    public string ClassCode { get; set; } = null!;
    public int LessonId { get; set; }
    public string LessonTitle { get; set; } = null!;
    public int EmployeeId { get; set; }
    public string EmployeeFullName { get; set; } = null!;
    public int Capacity { get; set; }
    public string ClassSchedule { get; set; } = null!;
    public int ClassLocationId { get; set; }
    public string ClassLocationName { get; set; } = null!;
    public int MajorId { get; set; }
    public string MajorName { get; set; } = null!;
    public DateTime FinalExamDate { get; set; }
    public int TermId { get; set; }
    public int ForEnteranceYearId { get; set; }
    public string ForEnteranceYearTitle { get; set; } = null!;
    public int LessonTypeId { get; set; }
    public string LessonTypeTitle { get; set; } = null!;
    public int Credit { get; set; }
}
