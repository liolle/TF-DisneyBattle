using blazor.models;
using blazor.services.state;
using blazor.utils;

namespace blazor.services;

public interface IConnectionManager
{
    public Task<bool> JoinQueueAsync(int playerId);
    public Task EndGame(GameMatch game);
}

public class ConnectionManager : IConnectionManager
{
    public OwnedSemaphore Searching_semaphore { get; } = new(1, 1);
    public OwnedSemaphore Player_poll_semaphore { get; } = new(1, 1);
    public OwnedSemaphore Match_semaphore { get; } = new(1, 1);
    public HashSet<int> Searching_poll { get; } = [];
    public Dictionary<int, PlayerConnectionContext> Player_poll { get; } = [];
    public Dictionary<int, GameMatch> Match_poll { get; } = [];

    public async Task<bool> JoinQueueAsync(int playerId)
    {
        await Searching_semaphore.WaitAsync();
        Searching_poll.Add(playerId);
        Searching_semaphore.Release();
        return true;
    }

    public async Task FindMatchUp()
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
        Searching_semaphore.Release();
    }

    private async Task CreateMatch(int playerId1, int playerId2)
    {
        await Player_poll_semaphore.WaitAsync();
        Player_poll.TryGetValue(playerId1, out PlayerConnectionContext? p1Context);
        Player_poll.TryGetValue(playerId2, out PlayerConnectionContext? p2Context);
        Player_poll_semaphore.Release();

        if (p1Context is null || p2Context is null)
        {
            p2Context?.Disconnect();
            p1Context?.Disconnect();
            return;
        }

        GameMatch match = new(playerId1, playerId2);
        
        await Match_semaphore.WaitAsync();
        Match_poll.Add(playerId1, match);
        Match_poll.Add(playerId2, match);
        Match_semaphore.Release();

        _ = p1Context.MatchFound(match);
        _ = p2Context.MatchFound(match);
    }

    public async Task EndGame(GameMatch match)
    {
        await Match_semaphore.WaitAsync();
        Match_poll.Remove(match.player1);
        Match_poll.Remove(match.player2);
        Match_semaphore.Release();

        await Player_poll_semaphore.WaitAsync();

        Player_poll.TryGetValue(match.player1, out PlayerConnectionContext? context_p1);
        Player_poll.TryGetValue(match.player2, out PlayerConnectionContext? context_p2);

        context_p1?.QuitGame();
        context_p2?.QuitGame();

        Player_poll_semaphore.Release();
    }
};