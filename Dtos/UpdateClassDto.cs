using System.ComponentModel.DataAnnotations;

namespace UniversitySystem3.Dtos;

public class UpdateClassDto
{
    public int LessonId { get; set; }
    public string ClassCode { get; set; } = null!;

    public string LessonTitle { get; set; } = null!;

    public int EmployeeID { get; set; }

    public int Capacity { get; set; }

    public string ClassSchedule { get; set; } = null!;

    public int ClassLocationId { get; set; }

    public DateTime FinalExamDate { get; set; }

    public int TermId { get; set; }

    public int ForEnteranceYearID { get; set; }

    public int LessonTypeId { get; set; }
    public int Credit { get; set; }


}
