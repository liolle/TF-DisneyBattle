using blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services.state;

public class PlayerConnectionContext
{
    private PlayerConnectionState _state = new EmptyState();
    private ConnectionManager _connectionManager;
    private IHubContext<ConnectionHub> _hub;

    public Player Player { get; set; }

    public void  UpdateHub(IHubContext<ConnectionHub> hub){
        _hub = hub;
    }

    public PlayerConnectionContext(PlayerConnectionState state, Player player, ConnectionManager connectionManager, IHubContext<ConnectionHub> clients)
    {
        _connectionManager = connectionManager;
        Player = player;
        _hub = clients;
        TransitionTo(state);
    }

    public bool IsSameType(Type type){
        return _state.GetType() == type;
    }

    public void TransitionTo(PlayerConnectionState state)
    {
        _state = state;
        _state.SetContext(this, _connectionManager, _hub);
    }

    public Task<bool> SearchGame()
    {
        return _state.SearchGame();
    }

    public Task<bool> MatchFound(GameMatch match)
    {
        return _state.MatchFound(match);
    }

    public Task<bool> Disconnect()
    {
        return _state.Disconnect();
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


    public virtual async Task<bool> Disconnect()
    {
        await Task.Delay(10);
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


    public virtual async Task<bool> SearchGame()
    {
        await Task.Delay(10);
        return false;
    }
}

public class EmptyState : PlayerConnectionState { }