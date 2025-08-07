using Shared.Domain.Enums;

namespace Shared.Domain.Interfaces;

/// <summary>
/// Contexto para acessar as informações do usuário.
/// </summary>
public interface IUserContext
{
    UserRoles Role { get; }
    string? TenantId { get; }
    bool IsAdmin { get; }

    /// <summary>
    /// Retorna informações sobre o contexto do usuário.
    /// </summary>
    /// <returns></returns>
    string GetInfo();
}