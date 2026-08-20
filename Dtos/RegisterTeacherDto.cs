using System.ComponentModel.DataAnnotations;

namespace UniversitySystem3.Dtos;

public class RegisterTeacherDto
{
    [Required]
    public string EmpCode { get; set; } = null!;
    [Required]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "کد ملی باید دقبقا 10 رقم باشد.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "کد ملی باید فقط شامل رقم باشد.")]
    public string NationalCode { get; set; } = null!;

    [Required]
    public string FullName { get; set; } = null!;

    public string? Title { get; set; }
    public string? LastDegree { get; set; }
}
