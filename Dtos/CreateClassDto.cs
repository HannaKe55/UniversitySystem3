using System.ComponentModel.DataAnnotations;

namespace UniversitySystem3.Dtos;

public class CreateClassDto
{
    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "کد کلاس باید دقیقاً ۶ رقم باشد.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "کد کلاس باید فقط شامل عدد باشد.")]
    public string ClassCode { get; set; } = null!;

    public int? LessonId { get; set; }
    public string? NewLessonTitle { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [Range(1, 500)]
    public int Capacity { get; set; }

    [Required]
    public string ClassSchedule { get; set; } = null!;

    [Required]
    public int ClassLocationId { get; set; }

    [Required]
    public DateTime FinalExamDate { get; set; }

    [Required]
    public int TermId { get; set; }

    [Required]
    public int ForEnteranceYearId { get; set; }

    [Required]
    public int LessonTypeId { get; set; }

    [Required]
    [Range(1, 6, ErrorMessage = "تعداد واحد باید بین ۱ تا ۶ باشد.")]
    public int Credit { get; set; }
}