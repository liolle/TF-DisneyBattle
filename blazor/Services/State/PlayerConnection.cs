using blazor.models;

namespace blazor.services.state;

public class PlayerConnectionContext 
{
    private PlayerConnectionState _state;
    public Player Player {get;set;}

    public PlayerConnectionContext(PlayerConnectionState state, Player player)
    {
        _state = state;
        Player = player;
    }

    public void TransitionTo(PlayerConnectionState state)
    {
        _state = state;
        _state.SetContext(this);
    }

    public bool SearchGame()
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
    public void SetContext(PlayerConnectionContext context)
    {
        _context = context;
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

    public bool SearchGame()
    {
        return false;
    }
}

public class PlayerSearching : PlayerConnectionState
{

}

public class PlayerMathFound : PlayerConnectionState
{

}

public class PlayerPlaying : PlayerConnectionState
{

}

public class PlayerLobby : PlayerConnectionState
{

}

public class PlayerTempDisconnection : PlayerConnectionState
{

}