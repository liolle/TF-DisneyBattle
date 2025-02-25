using blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services;

public static class ConnectionManager
{
    public static SemaphoreSlim Searching_semaphore { get; set; } = new(1, 1);
    public static SemaphoreSlim Playing_semaphore { get; set; } = new(1, 1);
    public static Dictionary<int, Player> Searching_poll { get; set; } = [];
    public static Dictionary<int, Player> Playing_poll { get; set; } = [];
    public static Dictionary<string, int> Connection_poll { get; set; } = [];
    public static Dictionary<int, GameMatch> Match_poll { get; set; } = [];
};


public class MatchHubService(IHubContext<MatchHubService> hubContext) : Hub, IMatchHubService
{

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public async Task AddPlayerAsync(Player player)
    {
        bool is_playing = await AddPlayingPlayerAsync(player);
        if (is_playing)
        {
            GameMatch? m = FindPlayerMatch(player.PlayerId);
            if (m is null) { return; }
            await NotifyMatchFoundAsync(m, player);
            return;
        }
        await ConnectionManager.Searching_semaphore.WaitAsync();
        ConnectionManager.Searching_poll.Add(player.PlayerId, player);
        ConnectionManager.Connection_poll.Add(player.connectionId, player.PlayerId);

        if (ConnectionManager.Searching_poll.Count < 2)
        {
            ConnectionManager.Searching_semaphore.Release();
            return;
        }
        ConnectionManager.Searching_semaphore.Release();

        GameMatch? match = await CreateGameAsync();
        if (match is null) { return; }
        await NotifyMatchFoundAsync(match, match.player1);
        await NotifyMatchFoundAsync(match, match.player2);
    }

    private async Task<bool> AddPlayingPlayerAsync(Player player)
    {
        try
        {

            await ConnectionManager.Playing_semaphore.WaitAsync();
            if (!ConnectionManager.Playing_poll.ContainsKey(player.PlayerId)) { return false; }
            ConnectionManager.Playing_poll.Add(player.PlayerId, player);
            ConnectionManager.Connection_poll.Add(player.connectionId, player.PlayerId);
            return true;
        }
        finally
        {
            ConnectionManager.Playing_semaphore.Release();
        }
    }

    private async Task<GameMatch> CreateMatch(Player player1, Player player2)
    {

        await ConnectionManager.Playing_semaphore.WaitAsync();

        ConnectionManager.Playing_poll.Add(player1.PlayerId, player1);
        ConnectionManager.Playing_poll.Add(player2.PlayerId, player2);

        GameMatch match = new(player1, player2);

        ConnectionManager.Match_poll.Add(player1.PlayerId, match);
        ConnectionManager.Match_poll.Add(player2.PlayerId, match);
        ConnectionManager.Playing_semaphore.Release();
        return match;
    }

    private async Task<GameMatch?> CreateGameAsync()
    {
        await ConnectionManager.Searching_semaphore.WaitAsync();
        if (ConnectionManager.Searching_poll.Count < 2)
        {
            ConnectionManager.Searching_semaphore.Release();
            return null;
        }

        Random random = new();
        await Task.Delay(random.Next(100) + 20);

        List<Player> players = [.. ConnectionManager.Searching_poll.Values];
        int idx1 = random.Next(players.Count);
        int idx2;
        do
        {
            idx2 = random.Next(players.Count);
        } while (idx1 == idx2);

        Player p1 = players[idx1];
        Player p2 = players[idx2];

        ConnectionManager.Searching_poll.Remove(p1.PlayerId);
        ConnectionManager.Searching_poll.Remove(p2.PlayerId);
        ConnectionManager.Searching_semaphore.Release();

        return await CreateMatch(p1, p2);
    }

    private async Task<bool> UpdateMatchConnection(GameMatch match)
    {

        try
        {
            await ConnectionManager.Playing_semaphore.WaitAsync();
            if (!match.UpdatePlayerConnection(ConnectionManager.Playing_poll)) { return false; }
            return true;
        }
        finally
        {
            ConnectionManager.Playing_semaphore.Release();
        }

    }

    public async Task NotifyMatchFoundAsync(GameMatch match, Player player)
    {
        bool success = await UpdateMatchConnection(match);
        if (!success) { return; }

        await hubContext.Clients.Client(player.ConId)
            .SendAsync("MatchFound", match,player);
    }

    public async Task RemovePlayerAsync(int playerId)
    {
        await ConnectionManager.Playing_semaphore.WaitAsync();
        ConnectionManager.Searching_poll.Remove(playerId);
        if (ConnectionManager.Playing_poll.ContainsKey(playerId))
        {
            ConnectionManager.Playing_semaphore.Release();
            return;
        }
        ConnectionManager.Playing_semaphore.Release();

        await ConnectionManager.Searching_semaphore.WaitAsync();
        ConnectionManager.Searching_poll.Remove(playerId);
        ConnectionManager.Searching_semaphore.Release();
    }

    public GameMatch? FindPlayerMatch(int playerId)
    {
        if (ConnectionManager.Match_poll.ContainsKey(playerId)) { return ConnectionManager.Match_poll[playerId]; }
        return null;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (!ConnectionManager.Connection_poll.TryGetValue(Context.ConnectionId, out int playerId)) { return; }
        await RemovePlayerAsync(playerId);
    }
}