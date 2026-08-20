namespace UniversitySystem3.Dtos;

public class StudentListItemDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = null!;

    public string StudentCode { get; set; } = null!;
    public string MajorName { get; set; } = null!;
}
