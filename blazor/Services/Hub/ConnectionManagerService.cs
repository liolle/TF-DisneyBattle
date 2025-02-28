using blazor.models;
using blazor.services.state;
using blazor.utils;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services;

public interface IConnectionManager
{
    public Task SearchGameAsync(int playerId, string connectionId);
    public Task JoinGame(int playerId);
}

public class ConnectionHub(ConnectionManager connectionManager, AuthenticationStateProvider authProvider, IHubContext<ConnectionHub> hubContext) : Hub
{

    public override async Task OnConnectedAsync()
    {
        var authState = await authProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        string? id_str = user.Claims.FirstOrDefault(val => val.Type == "Id")?.Value;
        if (id_str is null || !int.TryParse(id_str, out int id)) { return; }

        await connectionManager.Player_poll_semaphore.WaitAsync();
        if (!connectionManager.Player_poll.TryGetValue(id, out PlayerConnectionContext? context))
        {
            Player p = new(id, Context.ConnectionId);
            context = new(new PlayerLobby(), p, connectionManager, hubContext);
            connectionManager.Player_poll.Add(id, context);
        }
        connectionManager.Player_poll_semaphore.Release();
        _ = Clients.Client(Context.ConnectionId).SendAsync("hello");
    }
    public async Task SearchGameAsync(int playerId, string connectionId)
    {
        try
        {
            await connectionManager.Player_poll_semaphore.WaitAsync();
            // Get the player state context or create it
            if (!connectionManager.Player_poll.TryGetValue(playerId, out PlayerConnectionContext? context))
            {
                context = new(new PlayerLobby(), new Player(playerId, connectionId), connectionManager, hubContext);
                connectionManager.Player_poll.Add(playerId, context);
            }

            _ = context.SearchGame();
        }
        finally
        {
            connectionManager.Player_poll_semaphore.Release();
        }
    }

    public async Task NotifyMatchFound(GameMatch match, string connectionId)
    {
        await Clients.Client(connectionId).SendAsync("MatchFound", match);
    }
}

public class ConnectionManager
{
    public OwnedSemaphore Searching_semaphore { get; } = new(1, 1);
    public OwnedSemaphore Playing_semaphore { get; } = new(1, 1);
    public OwnedSemaphore Player_poll_semaphore { get; } = new(1, 1);
    public OwnedSemaphore Match_semaphore { get; } = new(1, 1);
    public HashSet<int> Searching_poll { get; } = [];
    public HashSet<int> Playing_poll { get; } = [];
    public Dictionary<int, PlayerConnectionContext> Player_poll { get; } = [];
    public Dictionary<int, GameMatch> Match_poll { get; } = [];

    public async Task<bool> JoinQueueAsync(int playerId)
    {
        try
        {
            await Searching_semaphore.WaitAsync();
            Searching_poll.Add(playerId);
            return true;
        }
        finally
        {
            Searching_semaphore.Release();
        }
    }

    public async Task FindMatchUp()
    {
        try
        {
            await Searching_semaphore.WaitAsync();
            if (Searching_poll.Count < 2) { return; }
            Random random = new();

            List<int> players = [.. Searching_poll];
            int p1 = random.Next(players.Count);
            int p2;
            do
            {
                p2 = random.Next(players.Count);
            } while (p1 == p2);

            int player1 = players[p1];
            int player2 = players[p2];

            Searching_poll.Remove(player1);
            Searching_poll.Remove(player2);
            await CreateMatch(player1, player2);
            return;
        }
        finally
        {
            Searching_semaphore.Release();
        }
    }

    private async Task CreateMatch(int playerId1, int playerId2)
    {
        await Player_poll_semaphore.WaitAsync();
        Player_poll.TryGetValue(playerId1, out PlayerConnectionContext? p1Context);
        Player_poll.TryGetValue(playerId2, out PlayerConnectionContext? p2Context);
        Player_poll_semaphore.Release();
        if (p1Context is null || p2Context is null)
        {
            // Add back into the searching_poll
            p2Context?.SearchDisconnect();
            p1Context?.SearchDisconnect();

            return;
        }

        GameMatch match = new(playerId1, playerId2);
        _=p1Context.MatchFound(match);
        _=p2Context.MatchFound(match);
    }

};

