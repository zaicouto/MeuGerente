namespace Shared.Domain.Interfaces;

/// <summary>
/// Contexto para acessar informações do inquilino (tenant).
/// </summary>
public interface ITenantContext
{
    string? TenantId { get; }

    /// <summary>
    /// Retorna informações sobre o contexto do locatário.
    /// </summary>
    /// <returns></returns>
    string GetInfo();
}
