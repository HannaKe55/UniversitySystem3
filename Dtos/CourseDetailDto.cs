namespace UniversitySystem3.Dtos;

public class CourseDetailDto
{
    public string ClassCode { get; set; } = null!;
    public string LessonTitle { get; set; } = null!;
    public int Credit { get; set; }
    public double? Score { get; set; }
    public string ResultText { get; set; } = null!;
    public string LessonTypeTitle { get; set; } = null!;
}
