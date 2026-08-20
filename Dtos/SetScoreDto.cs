//Professor panel - setting score for a specific student
using System.ComponentModel.DataAnnotations;
namespace UniversitySystem3.Dtos;


public class SetScoreDto
{
    [Required]
    [Range(0, 20, ErrorMessage = "نمره باید بین 0 و 20 باشد")]
    public double Score { get; set; }
}
