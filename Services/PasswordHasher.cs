namespace UniversitySystem3.Services;

public class PasswordHasher
{
    public static string Hash(string Password)
    {
        return BCrypt.Net.BCrypt.HashPassword(Password);
    }

    public static bool Verify(string Password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(Password, hashedPassword);
    }

}
