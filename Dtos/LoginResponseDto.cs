namespace UniversitySystem3.Dtos;

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public string Role { get; set; } = null!;  
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
}
