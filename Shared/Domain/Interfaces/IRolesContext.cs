using Shared.Domain.Enums;

namespace Shared.Domain.Interfaces;

/// <summary>
/// Contexto para acessar os papéis do usuário.
/// </summary>
public interface IRolesContext
{
    UserRoles Role { get; }
    bool IsAdmin { get; }
}
