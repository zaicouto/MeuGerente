using Modules.Users.Domain.Interfaces;

namespace Modules.Users.Infrastructure.Security;

/// <summary>
/// Implementação do hasher de senhas usando BCrypt.
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
