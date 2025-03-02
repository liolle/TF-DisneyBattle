
using blazor.models;
using Microsoft.JSInterop;

namespace blazor.services;

public class MatchService : IDisposable
{
    public string CurrentState { get; private set; } = "";
    public event Action<GameMatch, Player>? JoinGame;
    public event Func<Task>? OnStateChanged;
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<MatchService>? _dotNetObjectReference;

    public MatchService(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
        _dotNetObjectReference = DotNetObjectReference.Create(this);
        _jsRuntime.InvokeVoidAsync("initializeMatchService", _dotNetObjectReference);
    }


    [JSInvokable]
    public void NotifyJoinGame(GameMatch match, Player player)
    {
        CurrentState = "Playing";
        JoinGame?.Invoke(match, player);
        OnStateChanged?.Invoke();
    }

    [JSInvokable]
    public void NotifyLeftGame()
    {
        CurrentState = "";
        OnStateChanged?.Invoke();
    }

    public async Task SearchGameAsync(int playerId)
    {
        await _jsRuntime.InvokeVoidAsync("searchGame", playerId);
    }

    public void Dispose()
    {
        _dotNetObjectReference?.Dispose();
    }
}