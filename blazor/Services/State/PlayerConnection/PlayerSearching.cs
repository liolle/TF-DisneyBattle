using blazor.models;
using Microsoft.AspNetCore.SignalR;

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

        await Task.Delay(20);
        context.TransitionTo(new PlayerLobby());


        return true;
    }
}