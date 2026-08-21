namespace UniversitySystem3.Dtos;

public class TermListItemDto
{
    public int TermId { get; set; }
    public string TermCode { get; set; } = null!;
    public string TermTitle { get; set; } = null!;
    public string OddOreven { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int? EnteranceYear { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
