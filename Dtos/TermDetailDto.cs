namespace UniversitySystem3.Dtos;

public class TermDetailDto
{
    public string TermCode { get; set; } = null!;
    public List<CourseDetailDto> Courses { get; set; } = new();

    public int TakenCredits { get; set; }
    public int PassedCredits { get; set; }
    public int FailedCredits { get; set; }
    public double TermAverage { get; set; }
    public string ProbationStatus { get; set; } = null!;
}
