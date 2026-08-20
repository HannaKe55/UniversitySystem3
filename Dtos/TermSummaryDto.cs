namespace UniversitySystem3.Dtos;

public class TermSummaryDto
{
    public int TermId { get; set; }
    public string TermCode { get; set; } = null!;
    public int TakenCredits { get; set; }
    public int PassedCredits { get; set; }
    public int FailedCredits { get; set; }
    public double TermAverage { get; set; }
}
