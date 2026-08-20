using System.ComponentModel.DataAnnotations;

namespace UniversitySystem3.Dtos;

public class RegisterStudentDto
{
    public string StudentCode { get; set; } = null!;

    [Required]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "کد ملی باید 10 رقم باشد")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "کد ملی باید فقط شامل رقم باشد.")]
    public string NationalCode { get; set; } = null!;
    public string TermTitle { get; set; } = null!;

    public string? FullName { get; set; }
}
