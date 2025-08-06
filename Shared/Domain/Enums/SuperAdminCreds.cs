namespace Shared.Domain.Enums;

/// <summary>
/// Credenciais do Super Admin.
/// </summary>
public static class SuperAdminCreds
{
    public const string FirstName = "Admin";
    public const string LastName = "Admin";
    public const string Email = "admin@email.com";
    public const string Password = "Pass@12345";
    public const string TenantId = "688d84098c4a334a6a5f0afa";
    public const UserRoles Role = UserRoles.SuperAdmin;

    /// <summary>
    /// Valida as credenciais do Super Admin.
    /// </summary>
    /// <param name="email">E-mail de super admin.</param>
    /// <param name="password">Senha de super admin.</param>
    /// <returns>True ou false.</returns>
    public static bool IsValid(string email, string password)
    {
        return email == Email && password == Password;
    }

    /// <summary>
    /// Valida se é o e-mail do Super Admin.
    /// </summary>
    /// <param name="email"></param>
    /// <returns>True ou false.</returns>
    public static bool IsValidEmail(string email)
    {
        return email == Email;
    }

    /// <summary>
    /// Valida se é a senha do Super Admin.
    /// </summary>
    /// <param name="password"></param>
    /// <returns>True ou false.</returns>
    public static bool IsValidPassword(string password)
    {
        return password == Password;
    }
}
