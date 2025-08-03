namespace Shared.Interfaces;

public interface ITenantContext
{
    string? TenantId { get; }
    string GetInfo();
}
