
using blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services;

public class MatchService
{
    public event Action<GameMatch, Player>? MatchFound;
    private readonly IHubContext<MatchHubService> _hubContext;

    public MatchService(IHubContext<MatchHubService> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task JoinMatchmakingAsync(int playerId, string connectionId)
    {

        await _hubContext.Clients.Client(connectionId).SendAsync("AddPlayerAsync", new Player(playerId, connectionId));
    }

    public void NotifyMatchFound(GameMatch match, Player player)
    {
        
        MatchFound?.Invoke(match, player);
    }
}