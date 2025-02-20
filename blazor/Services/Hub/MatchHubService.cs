using blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace blazor.services;


public class MatchHubService(IHubContext<MatchHubService> hubContext, MatchService matchService) : Hub, IMatchHubService
{

    private readonly SemaphoreSlim _searching_semaphore = new(1, 1);
    private readonly SemaphoreSlim _playing_semaphore = new(1, 1);
    private readonly Dictionary<int, Player> _searching_poll = [];
    private readonly Dictionary<int, Player> _playing_poll = [];
    private readonly Dictionary<string, int> _connection_poll = [];
    private readonly Dictionary<int, GameMatch> _match_poll = [];

    private readonly IHubContext<MatchHubService> _hubContext = hubContext;

    public async Task AddPlayerAsync(Player player)
    {
        Console.WriteLine(Context.ConnectionId);
        bool is_playing = await AddPlayingPlayerAsync(player);
        if (is_playing)
        {
            GameMatch? m = FindPlayerMatch(player.Id);
            if (m is null) { return; }
            await NotifyMatchFoundAsync(m, player);
            return;
        }

        await _searching_semaphore.WaitAsync();
        _searching_poll.Add(player.Id, player);
        _connection_poll.Add(player.connectionId, player.Id);
        _searching_semaphore.Release();

        if (_searching_poll.Count < 2) { return; }

        GameMatch? match = await CreateGameAsync();
        if (match is null) { return; }
        await NotifyMatchFoundAsync(match, match.player1);
        await NotifyMatchFoundAsync(match, match.player1);
    }

    private async Task<bool> AddPlayingPlayerAsync(Player player)
    {
        try
        {
            await _playing_semaphore.WaitAsync();
            if (!_playing_poll.ContainsKey(player.Id)) { return false; }
            _playing_poll.Add(player.Id, player);
            _connection_poll.Add(player.connectionId, player.Id);
            return true;
        }
        finally
        {
            _playing_semaphore.Release();
        }
    }

    private async Task<GameMatch> CreateMatch(Player player1, Player player2)
    {

        await _playing_semaphore.WaitAsync();

        _playing_poll.Add(player1.Id, player1);
        _playing_poll.Add(player2.Id, player2);

        GameMatch match = new(player1, player2);

        _match_poll.Add(player1.Id, match);
        _match_poll.Add(player2.Id, match);
        _playing_semaphore.Release();
        return match;
    }

    private async Task<GameMatch?> CreateGameAsync()
    {
        await _searching_semaphore.WaitAsync();
        if (_searching_poll.Count < 2)
        {
            _searching_semaphore.Release();
            return null;
        }

        Random random = new();
        await Task.Delay(random.Next(100) + 20);

        List<Player> players = [.. _searching_poll.Values];
        int idx1 = random.Next(players.Count);
        int idx2;
        do
        {
            idx2 = random.Next(players.Count);
        } while (idx1 == idx2);

        Player p1 = players[idx1];
        Player p2 = players[idx2];

        _searching_poll.Remove(p1.Id);
        _searching_poll.Remove(p2.Id);
        _searching_semaphore.Release();

        return await CreateMatch(p1, p2);
    }

    private async Task<bool> UpdateMatchConnection(GameMatch match)
    {

        try
        {
            await _playing_semaphore.WaitAsync();
            if (!match.UpdatePlayerConnection(_playing_poll)) { return false; }
            return true;
        }
        finally
        {
            _playing_semaphore.Release();
        }

    }

    public async Task NotifyMatchFoundAsync(GameMatch match, Player player)
    {
        bool success = await UpdateMatchConnection(match);
        if (!success) { return; }

        await _hubContext.Clients.Client(match.Player1.ConnectionId)
            .SendAsync("MatchFound", player.Id);

        matchService.NotifyMatchFound(match,player);
    }

    public async Task RemovePlayerAsync(int playerId)
    {
        await _playing_semaphore.WaitAsync();
        _searching_poll.Remove(playerId);
        if (_playing_poll.ContainsKey(playerId))
        {
            _playing_semaphore.Release();
            return;
        }
        _playing_semaphore.Release();

        await _searching_semaphore.WaitAsync();
        _searching_poll.Remove(playerId);
        _searching_semaphore.Release();
    }

    public GameMatch? FindPlayerMatch(int playerId)
    {
        if (_match_poll.ContainsKey(playerId)) { return _match_poll[playerId]; }
        return null;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        int playerId = _connection_poll[Context.ConnectionId];
        await RemovePlayerAsync(playerId);
        await base.OnDisconnectedAsync(exception);
    }
}