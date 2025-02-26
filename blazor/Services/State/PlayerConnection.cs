using System.Threading.Tasks;
using blazor.models;

namespace blazor.services.state;

public class PlayerConnectionContext
{
    private PlayerConnectionState _state;
    private ConnectionManager _connectionManager;

    public Player Player { get; set; }

    public PlayerConnectionContext(PlayerConnectionState state, Player player,ConnectionManager connectionManager)
    {
        _state = state;
        _connectionManager = connectionManager;
        Player = player;
        
    }

    public void TransitionTo(PlayerConnectionState state)
    {
        _state = state;
        _state.SetContext(this,_connectionManager);
    }

    public Task<bool> SearchGame()
    {
        return _state.SearchGame();
    }

    public bool MatchFound()
    {
        return _state.MatchFound();
    }

    public bool SearchDisconnect()
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

    public bool JoinGame()
    {
        return _state.JoinGame();
    }

}

public abstract class PlayerConnectionState
{
    protected PlayerConnectionContext? _context;
    protected ConnectionManager? _connectionManager;
    public void SetContext(PlayerConnectionContext context,ConnectionManager connectionManager)
    {
        _context = context;
        _connectionManager = connectionManager;
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

    public bool JoinGame()
    {
        return false;
    }

    public bool MatchFound()
    {
        return false;
    }

    public bool SearchDisconnect()
    {
        return false;
    }

    public virtual async Task<bool> SearchGame()
    {
        await Task.Delay(10);
        return false;
    }
}

public class PlayerSearching : PlayerConnectionState
{
    public PlayerSearching(){
        Console.WriteLine(_context?.Player.ToString() ?? "No player");
    }

}

public class PlayerMathFound : PlayerConnectionState
{

}

public class PlayerPlaying : PlayerConnectionState
{

}

public class PlayerLobby : PlayerConnectionState
{

    public override async Task<bool> SearchGame()
    {
        PlayerConnectionContext? context = base._context;
        ConnectionManager? connectionManager = base._connectionManager;
        if (context is null || connectionManager is null){return false;}
        await connectionManager.JoinQueueAsync(context.Player.id);
        context.TransitionTo(new PlayerSearching());
        return true;
    }

}

public class PlayerTempDisconnection : PlayerConnectionState
{

}