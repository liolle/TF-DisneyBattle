using System.Security.Claims;
using blazor.models;
using blazor.services.state;
using blazor.utils;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services;

public class ConnectionHub(ConnectionManager connectionManager, AuthenticationStateProvider authProvider, IHubContext<ConnectionHub> hubContext) : Hub
{

    private async Task<IEnumerable<Claim>> GetClaims()
    {
        return (await authProvider.GetAuthenticationStateAsync()).User.Claims;
    }

    public override async Task OnConnectedAsync()
    {
        IEnumerable<Claim> claims = await GetClaims();
        int? id = Utils.ExtractIntFromClaim(claims, "Id");
        if (id is null) { return; }

        Player p = new(id.Value, Context.ConnectionId);

        await connectionManager.Player_poll_semaphore.WaitAsync();
        if (!connectionManager.Player_poll.TryGetValue(id.Value, out PlayerConnectionContext? context))
        {
            context = new(new PlayerLobby(), p, connectionManager, hubContext);
            connectionManager.Player_poll.Add(id.Value, context);
        }
        else
        {
            context.Player = p;
        }
        connectionManager.Player_poll_semaphore.Release();

        if (!context.IsSameType(typeof(PlayerLobby)))
        {
            await context.SearchGame();
        }
    }

    public async Task SearchGameAsync(int playerId, string connectionId)
    {
        await connectionManager.Player_poll_semaphore.WaitAsync();
        if (!connectionManager.Player_poll.TryGetValue(playerId, out PlayerConnectionContext? context))
        {
            context = new(new PlayerLobby(), new Player(playerId, connectionId), connectionManager, hubContext);
            connectionManager.Player_poll.Add(playerId, context);
        }
        _ = context.SearchGame();
        connectionManager.Player_poll_semaphore.Release();
    }

    public async Task LeaveGameAsync(int playerId)
    {
        await connectionManager.Player_poll_semaphore.WaitAsync();
        connectionManager.Player_poll.TryGetValue(playerId, out PlayerConnectionContext? context);
        _ = context?.Quit();
        connectionManager.Player_poll_semaphore.Release();
    }


    public async Task<GameMatch?> GetGameState()
    {
        IEnumerable<Claim> claims = await GetClaims();
        int? id = Utils.ExtractIntFromClaim(claims, "Id");
        if (id is null) { return null; }
        connectionManager.Match_poll.TryGetValue(id.Value, out GameMatch? match);
        return match;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        IEnumerable<Claim> claims = await GetClaims();

        int? id = Utils.ExtractIntFromClaim(claims, "Id");
        if (id is null) { return; }

        Player p = new(id.Value, Context.ConnectionId);
        await connectionManager.Player_poll_semaphore.WaitAsync();
        if (!connectionManager.Player_poll.TryGetValue(id.Value, out PlayerConnectionContext? context))
        {
            context = new(new PlayerLobby(), p, connectionManager, hubContext);
            connectionManager.Player_poll.Add(id.Value, context);
        }
        connectionManager.Player_poll_semaphore.Release();

        await context.Disconnect();
        await base.OnDisconnectedAsync(exception);
    }
}