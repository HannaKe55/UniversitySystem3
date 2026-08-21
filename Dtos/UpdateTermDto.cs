namespace UniversitySystem3.Dtos;

public class UpdateTermDto
{
    public string TermTitle { get; set; } = null!;
    public string OddOreven { get; set; } = null!;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string Status { get; set; } = null!;
    public int? EnteranceYear { get; set; }
}
