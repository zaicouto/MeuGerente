namespace Shared.Domain.Enums;

public static class SuperAdminCreds
{
    public const string FirstName = "Admin";
    public const string LastName = "Admin";
    public const string Email = "admin@email.com";
    public const string Password = "Pass@12345";
    public const string TenantId = "688d84098c4a334a6a5f0afa";
    public const UserRoles Role = UserRoles.SuperAdmin;

    public static bool IsValid(string username, string password)
    {
        return username == Email && password == Password;
    }

    public static bool IsValidUsername(string username)
    {
        return username == Email;
    }

    public static bool IsValidPassword(string password)
    {
        return password == Password;
    }
}
