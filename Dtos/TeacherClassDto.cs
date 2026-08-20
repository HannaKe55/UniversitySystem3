// Professor Panel - List of Classes in the current term
namespace UniversitySystem3.Dtos;

public class TeacherClassDto
{
    public int ClassId { get; set; }
    public string ClassCode { get; set; } = null!;
    public string LessonTitle { get; set; } = null!;
    public string MajorName { get; set; } = null!;
    public string EnteranceYearTitle { get; set; } = null!;
    public string TermCode { get; set; } = null!;

}
