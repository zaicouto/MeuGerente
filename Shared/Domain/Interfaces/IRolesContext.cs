using Shared.Domain.Enums;

namespace Shared.Domain.Interfaces;

public interface IRolesContext
{
    UserRoles Role { get; }
    bool IsAdmin { get; }
}
