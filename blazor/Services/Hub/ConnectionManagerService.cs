using blazor.models;
using blazor.services.state;
using blazor.utils;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services;

public interface IConnectionManager
{
    public Task SearchGameAsync(int playerId, string connectionId);
}

public class ConnectionHub(ConnectionManager connectionManager) : Hub
{
    public async Task SearchGameAsync(int playerId, string connectionId)
    {
        try
        {
            await connectionManager.Player_poll_semaphore.WaitAsync();
            // Get the player state context or create it
            if (!connectionManager.Player_poll.TryGetValue(playerId, out PlayerConnectionContext? context))
            {
                PlayerConnectionContext playerConnectionContext = new(new PlayerLobby(), new Player(playerId, connectionId), connectionManager);
                context = playerConnectionContext;
                connectionManager.Player_poll.Add(playerId, context);
            }

            _ = context.SearchGame();
        }
        finally
        {
            connectionManager.Player_poll_semaphore.Release();
        }
    }
}

public partial class ConnectionManager 
{
    public OwnedSemaphore Searching_semaphore { get; } = new(1, 1);
    public OwnedSemaphore Playing_semaphore { get; } = new(1, 1);
    public OwnedSemaphore Player_poll_semaphore { get; } = new(1, 1);
    public OwnedSemaphore Match_semaphore { get; } = new(1, 1);
    public HashSet<int> Searching_poll { get; } = [];
    public HashSet<int> Playing_poll { get; } = [];
    public Dictionary<int, PlayerConnectionContext> Player_poll { get; } = [];
    public Dictionary<int, GameMatch> Match_poll { get; } = [];

    public ConnectionManager()
    {
        Console.WriteLine("Regenerate");
    }


};

public partial class ConnectionManager
{

    public async Task<bool> JoinQueueAsync(int playerId)
    {
        try
        {
            await Searching_semaphore.WaitAsync();
            if (Searching_poll.Count < 2) { return false; }
            Random random = new();

            List<int> players = [.. Searching_poll];
            int p1 = random.Next(players.Count);
            int p2;
            do
            {
                p2 = random.Next(players.Count);
            } while (p1 == p2);

            Searching_poll.Remove(p1);
            Searching_poll.Remove(p2);
            return true;
        }
        finally
        {
            Searching_semaphore.Release();
        }
    }
    private async Task FindMatchUp()
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

            Searching_poll.Remove(p1);
            Searching_poll.Remove(p2);
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
            // send back to the lobby
            p2Context?.SearchDisconnect();
            p1Context?.SearchDisconnect();

            return;
        }

        p2Context.MatchFound();
        p1Context.MatchFound();
    }

    public async Task<bool> NotifyMatchFound(GameMatch match)
    {

        try
        {
            await Player_poll_semaphore.WaitAsync();
            Player_poll.TryGetValue(match.P1, out PlayerConnectionContext? p1Context);
            Player_poll.TryGetValue(match.P2, out PlayerConnectionContext? p2Context);
            Player_poll_semaphore.Release();

            if (p1Context is null || p2Context is null)
            {
                // send back to the lobby
                return false;
            }

            /*

                        await _hubContext.Clients.Client(p1Context.Player.ConId)
                            .SendAsync("MatchFound", match);

                        await _hubContext.Clients.Client(p2Context.Player.ConId)
                            .SendAsync("MatchFound", match);
            */

            return true;
        }
        finally
        {
            Player_poll_semaphore.Release();
        }

    }
}
