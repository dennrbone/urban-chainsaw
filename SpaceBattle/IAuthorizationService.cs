namespace SpaceBattle;

public interface IAuthorizationService
{
    void Authorize(string playerId, string action);
}