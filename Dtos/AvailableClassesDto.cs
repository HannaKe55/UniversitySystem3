namespace UniversitySystem3.Dtos;

public class AvailableClassesDto
{
    public int ClassId { get; set; }
    public string ClassCode { get; set; } = null!;

    public string LessonTitle { get; set; } = null!;
    public string EmployeeFullName { get; set; } = null!;
    public string MajorName { get; set; } = null!;
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public string LessonTypeTitle { get; set; } = null!;
    public int Credit { get; set; }

}
