namespace UniversitySystem3.Dtos;

public class StudentCourseSelectionDto
{
    public string FullName { get; set; } = null!;
    public string StudentCode { get; set; } = null!;
    public string EnteranceYearTitle { get; set; } = null!;
    public double PreviousTermAverage { get; set; }

    public List<SelectedClassDto> SelectedClasses { get; set; } = new();
}
