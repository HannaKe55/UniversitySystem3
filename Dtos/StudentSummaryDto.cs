namespace UniversitySystem3.Dtos;

public class StudentSummaryDto
{
    public string FullName { get; set; } = null!;
    public string StudentCode { get; set; } = null!;
    public string MajorName { get; set; } = null!;
    public string EnteranceYearTitle { get; set; } = null!;
    public int TotalPassedCredits { get; set; }
    public double OverallAverage { get; set; }

    public List<TermSummaryDto> Terms { get; set; } = new();
}
