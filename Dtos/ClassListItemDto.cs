namespace UniversitySystem3.Dtos;

public class ClassListItemDto
{
    public int ClassId { get; set; }
    public string ClassCode { get; set; } = null!;
    public string LessonTitle { get; set; } = null!;
    public string EmployeeFullName { get; set; } = null!;
    public string MajorName { get; set; } = null!;
}
