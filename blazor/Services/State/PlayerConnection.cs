using blazor.models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services.state;

public class PlayerConnectionContext
{
    private PlayerConnectionState _state = new EmptyState();
    private ConnectionManager _connectionManager;
    private IHubContext<ConnectionHub> _clients;

    public Player Player { get; set; }

    public PlayerConnectionContext(PlayerConnectionState state, Player player, ConnectionManager connectionManager, IHubContext<ConnectionHub> clients)
    {
        _connectionManager = connectionManager;
        Player = player;
        _clients = clients;
        TransitionTo(state);
    }

    public void TransitionTo(PlayerConnectionState state)
    {
        _state = state;
        _state.SetContext(this, _connectionManager, _clients);
    }

    public Task<bool> SearchGame()
    {
        return _state.SearchGame();
    }

    public Task<bool> MatchFound(GameMatch match)
    {
        return _state.MatchFound(match);
    }

    public Task<bool> SearchDisconnect()
    {
        return _state.SearchDisconnect();
    }

    public bool FoundDisconnect()
    {
        return _state.FoundDisconnect();
    }

    public bool GameDisconnect()
    {
        return _state.GameDisconnect();
    }

    public bool GameReconnect()
    {
        return _state.GameReconnect();
    }

    public bool FoundReconnect()
    {
        return _state.FoundReconnect();
    }

    public Task<bool> JoinGame()
    {
        return _state.JoinGame();
    }

}

public abstract class PlayerConnectionState
{
    protected PlayerConnectionContext? _context;
    protected ConnectionManager? _connectionManager;
    protected IHubContext<ConnectionHub>? _clients;

    public PlayerConnectionState()
    {
        _ = AfterInit();
    }

    public async virtual Task AfterInit()
    {
        await Task.Delay(20);
    }

    public void SetContext(PlayerConnectionContext context, ConnectionManager connectionManager, IHubContext<ConnectionHub> clients)
    {
        _context = context;
        _connectionManager = connectionManager;
        _clients = clients;
    }

    public bool FoundDisconnect()
    {
        return false;
    }

    public bool FoundReconnect()
    {
        return false;
    }

    public bool GameDisconnect()
    {
        return false;
    }

    public bool GameReconnect()
    {
        return false;
    }

    public virtual async Task<bool> JoinGame()
    {
        await Task.Delay(10);
        return false;
    }

    public virtual async Task<bool> MatchFound(GameMatch match)
    {
        await Task.Delay(10);
        return false;
    }

    public virtual async Task<bool> SearchDisconnect()
    {
        await Task.Delay(10);
        return false;
    }

    public virtual async Task<bool> SearchGame()
    {
        await Task.Delay(10);
        return false;
    }
}

public class EmptyState : PlayerConnectionState { }

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
        }
    }


    public override async Task<bool> MatchFound(GameMatch match)
    {
        await Task.Delay(10);
        PlayerConnectionContext? context = _context;

        if (context is null ) { return false; }
       
        context.TransitionTo(new PlayerMathFound(match));

        return false;
    }

    public override async Task<bool> SearchDisconnect()
    {
        await Task.Delay(10);
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;

        if (context is null || connectionManager is null) { return false; }
        Player player = context.Player;
        if (player is null) { return false; }

        context.TransitionTo(new PlayerLobby());


        return false;
    }
}

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
        if (context is null || connectionManager is null || clients is null) { return; }

        clients.Clients.Client(context.Player.connectionId)
            .SendAsync("MatchFound", match).GetAwaiter().OnCompleted(() =>
            {
                Console.WriteLine($"Player {context.Player.id} Found a match \n- {match}");
                //context.JoinGame();
            });
    }

    public override async Task<bool> JoinGame()
    {
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;
        if (context is null || connectionManager is null) { return false; }
        await connectionManager.Playing_semaphore.WaitAsync();
        connectionManager.Playing_poll.Add(context.Player.id);
        connectionManager.Playing_semaphore.Release();
        context.TransitionTo(new PlayerPlaying());
        return true;
    }
}

public class PlayerPlaying : PlayerConnectionState
{

    public override async Task AfterInit()
    {
        await base.AfterInit();
        Console.WriteLine($"Player {_context?.Player.id} Is Playing");
    }
}

public class PlayerLobby : PlayerConnectionState
{

    public override async Task AfterInit()
    {
        await base.AfterInit();
        Console.WriteLine($"Player {_context?.Player.id} Entered the Lobby");
    }

    public override async Task<bool> SearchGame()
    {
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;
        if (context is null || connectionManager is null) { return false; }
        await connectionManager.JoinQueueAsync(context.Player.id);
        context.TransitionTo(new PlayerSearching());
        _ = connectionManager.FindMatchUp();

        return true;
    }

}

public class PlayerTempDisconnection : PlayerConnectionState
{

}