using System.Collections.Generic;

namespace SpaceBattle;

public class AuthorizationService : IAuthorizationService
{
    private readonly HashSet<(string PlayerId, string Action)> _allowedPermissions;

    public AuthorizationService(IEnumerable<(string PlayerId, string Action)> allowedPermissions)
    {
        _allowedPermissions = new HashSet<(string PlayerId, string Action)>(allowedPermissions);
    }

    public void Authorize(string playerId, string action)
    {
        if (!_allowedPermissions.Contains((playerId, action)))
        {
            throw new UnauthorizedException($"Player '{playerId}' is unauthorized for action '{action}'.");
        }
    }
}