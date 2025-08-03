using Shared.Enums;

namespace Shared.Interfaces;

public interface IRolesContext
{
    UserRoles Role { get; }
    bool IsAdmin { get; }
}
