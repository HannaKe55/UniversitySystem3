namespace UniversitySystem3.Dtos;

public class StudentYearGroupDto
{
    public string EnteranceYear { get; set; } = null!;

    public List<StudentListItemDto> Students { get; set; } = new();
}
