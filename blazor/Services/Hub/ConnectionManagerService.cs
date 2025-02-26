using blazor.models;
using blazor.services.state;
using blazor.utils;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services;

public interface IConnectionManager
{
    public Task SearchGameAsync(int playerId);
}

public partial class ConnectionManager(IHubContext<ConnectionManager> hubContext) : Hub, IConnectionManager
{
    private OwnedSemaphore Searching_semaphore = new(1, 1);
    private OwnedSemaphore Playing_semaphore = new(1, 1);
    private OwnedSemaphore Player_poll_semaphore = new(1, 1);
    private OwnedSemaphore Match_semaphore = new(1, 1);
    private HashSet<int> Searching_poll = [];
    private HashSet<int> Playing_poll = [];
    private Dictionary<int, PlayerConnectionContext> Player_poll = [];
    private Dictionary<int, GameMatch> Match_poll = [];



    public async Task SearchGameAsync(int playerId)
    {
        try
        {
            await Player_poll_semaphore.WaitAsync();
            // Get the player state context or create it
            if (!Player_poll.TryGetValue(playerId, out PlayerConnectionContext? context))
            {
                PlayerConnectionContext playerConnectionContext = new(new PlayerSearching(), new Player(playerId,Context.ConnectionId));
                context = playerConnectionContext;
                Player_poll.Add(playerId, context);
            }

            // Not in a valid state to search for a game.
            if (!context.SearchGame()) { return; }

            await Searching_semaphore.WaitAsync();
            Searching_poll.Add(playerId);
            return;
        }
        finally
        {
            Searching_semaphore.Release();
            Player_poll_semaphore.Release();
        }
    }
};

public partial class ConnectionManager
{
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


            await hubContext.Clients.Client(p1Context.Player.ConId)
                .SendAsync("MatchFound", match);

            await hubContext.Clients.Client(p2Context.Player.ConId)
                .SendAsync("MatchFound", match);

            return true;
        }
        finally
        {
            Player_poll_semaphore.Release();
        }

    }
}
