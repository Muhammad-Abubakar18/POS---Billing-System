namespace DrMusa.Common.Utilities;

public static class PasswordHelper
{
    /// <summary>Hashes a plain-text password using BCrypt-style SHA256 + salt for simplicity.</summary>
    public static string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password + "DrMusa_Salt_2024");
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string hash)
        => HashPassword(password) == hash;
}
