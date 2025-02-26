
using blazor.models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.JSInterop;

namespace blazor.services;

public class MatchService : IDisposable
{
    public event Action<GameMatch, Player>? MatchFound;
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<MatchService>? _dotNetObjectReference;

    public MatchService(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

    public async Task InitializeAsync()
    {
        // Create a .NET object reference
        _dotNetObjectReference = DotNetObjectReference.Create(this);

        // Pass the reference to JavaScript
        await _jsRuntime.InvokeVoidAsync("initializeMatchService", _dotNetObjectReference);
    }

    [JSInvokable]
    public void NotifyMatchFound(GameMatch match, Player player)
    {
        MatchFound?.Invoke(match, player);
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