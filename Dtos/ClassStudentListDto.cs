
// professor panel - representing the student list of a specific class
namespace UniversitySystem3.Dtos;

public class ClassStudentListDto
{
    public string LessonTitle { get; set; } = null!;
    public string TermCode { get; set; } = null!;
    public List<ClassStudentDto> Students { get; set; } = new();
}

public class ClassStudentDto
{
    public int CourseRegId { get; set; }
    public int StudentId { get; set; }

    public string FullName { get; set; } = null!;
    public string StudentCode { get; set; } = null!;

    public string MajorName { get; set; } = null!;

}