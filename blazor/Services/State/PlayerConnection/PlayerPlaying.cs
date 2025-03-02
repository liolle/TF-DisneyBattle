using blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services.state;

public class PlayerPlaying : PlayerConnectionState
{
    private readonly GameMatch match;

    public PlayerPlaying(GameMatch match)
    {
        this.match = match;
    }

    public override async Task AfterInit()
    {
        await base.AfterInit();
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;
        IHubContext<ConnectionHub>? hub = _clients;
        if (context is null || connectionManager is null || hub is null) { return; }

        hub.Clients.Client(context.Player.connectionId)
            .SendAsync("Join_game", match, context.Player).GetAwaiter().OnCompleted(() =>
            {
                Console.WriteLine($"Player {_context?.Player} Is Playing");
            });
    }

    public override async Task<bool> Disconnect()
    {
        await Task.Delay(50);
        PlayerConnectionContext? context = _context;
        if (context is null ) { return false; }
        context.TransitionTo(new PlayerTempDisconnection(match));
        return true;
    }


    public override async Task<bool> Quit(){
        await base.AfterInit();
        ConnectionManager? connectionManager = _connectionManager;
        if ( connectionManager is null ) { return false; }

        await connectionManager.EndGame(match);

        return true;
    }

}