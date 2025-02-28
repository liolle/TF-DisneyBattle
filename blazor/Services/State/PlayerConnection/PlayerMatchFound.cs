using blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services.state;

public class PlayerMathFound : PlayerConnectionState
{
    private readonly GameMatch match;

    public PlayerMathFound(GameMatch match)
    {
        this.match = match;
    }

    public override async Task AfterInit()
    {
        await base.AfterInit();
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;
        IHubContext<ConnectionHub>? clients = _clients;
        if (context is null || connectionManager is null || clients is null)
        {
            return;
        }
        Console.WriteLine($"Player {context.Player.id} Found a match \n- {match}");
        await JoinGame();
    }

    public override async Task<bool> MatchFound(GameMatch match)
    {
        Console.WriteLine("Found");
        await Task.Delay(10);

        return false;
    }

    public override async Task<bool> JoinGame()
    {
        await Task.Delay(50);
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;
        if (context is null || connectionManager is null) { return false; }

        context.TransitionTo(new PlayerPlaying(match));
        return true;
    }

    public override async Task<bool> Disconnect()
    {
        await Task.Delay(50);
        PlayerConnectionContext? context = _context;
        if (context is null ) { return false; }
        context.TransitionTo(new PlayerTempDisconnection());
        return true;
    }
}

