using blazor.models;

namespace blazor.services;

public interface IMatchHubService
{
    public Task AddPlayerAsync(Player player);
    public Task RemovePlayerAsync(int playerId);
    public Task NotifyMatchFoundAsync(GameMatch match, Player player);
    public GameMatch? FindPlayerMatch(int playerId);
}