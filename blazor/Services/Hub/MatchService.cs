
using blazor.models;
using Microsoft.JSInterop;

namespace blazor.services;

public class MatchService : IDisposable
{
    public event Action<GameMatch, Player>? JoinGame;
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
        JoinGame?.Invoke(match, player);
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