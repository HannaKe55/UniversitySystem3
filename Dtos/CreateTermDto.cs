namespace UniversitySystem3.Dtos;

public class CreateTermDto
{
    public string TermCode { get; set; } = null!;
    public string TermTitle { get; set; } = null!;
    public string? OddOreven { get; set; } = null!;  
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string Status { get; set; } = "فعال";  
    public int? EnteranceYear { get; set; }
}
