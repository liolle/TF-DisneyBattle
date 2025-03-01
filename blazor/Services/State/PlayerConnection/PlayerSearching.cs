using blazor.models;
namespace blazor.services.state;

public class PlayerSearching : PlayerConnectionState
{
    public override async Task AfterInit()
    {
        await base.AfterInit();

        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;
        if (context is null || connectionManager is null) { return; }

        bool joined = await connectionManager.JoinQueueAsync(context.Player.id);
        if (joined)
        {
            Console.WriteLine($"Player {context.Player.id} Is searching for a match");
            // Random delay to prioritize disconnections.
            // the idea is to process disconnections before picking player from the search_poll.
            // probably need some sort of scheduler using a priority queue (see after).

            Random random = new();
            int delay = random.Next(500)+500;
            await Task.Delay(delay);
            _=connectionManager.FindMatchUp();
        }

    }


    public override async Task<bool> MatchFound(GameMatch match)
    {
        await Task.Delay(20);
        PlayerConnectionContext? context = _context;

        if (context is null)
        {
            return false;
        }
        context.TransitionTo(new PlayerMathFound(match));

        return true;
    }

    public override async Task<bool> Disconnect()
    {
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;

        if (context is null || connectionManager is null) { return false; }
        Player player = context.Player;
        if (player is null) { return false; }
        await connectionManager.Searching_semaphore.WaitAsync();
        connectionManager.Searching_poll.Remove(player.id);
        connectionManager.Searching_semaphore.Release();
        context.TransitionTo(new PlayerLobby());
        return true;
    }
}