using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace blazor.Components.Pages.Game;


public partial class Game : ComponentBase, IDisposable
{

    [Inject]
    MatchService? _matchService { get; set; }

    [Inject]
    private AuthenticationStateProvider? AuthProvider { get; set; }

    public GameMatch? _game { get; private set; } = new GameMatch(0, 0);

    public Player? _me { get; private set; }
    public Player? _opponent { get; private set; }

    private void HandleGameStateChange(GameMatch game)
    {
        _game = game;
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(10);
        if (_matchService is null) { return; }
        _matchService.OnGameChange += HandleGameStateChange;
    }

    private async Task SetPlayers()
    {

        if (AuthProvider is null || _game is null) { return; }
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        string? id_str = user.FindFirst("Id")?.Value;
        if (id_str is null){return;}
        int id = int.Parse(id_str);
        _me = new(id, "");
        if (_game.player1 == id)
        {
            _opponent = new(_game.player2, "");
        }
        else
        {
            _opponent = new(_game.player1, "");
        }
        StateHasChanged();
    }


    private void LeaveGame(){
        if (_me is null) {return;} 
        _=_matchService?.LeaveGameAsync(_me.id);
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_matchService is null) { return; }
        if (firstRender)
        {
            GameMatch? g = await _matchService.GetGameStateAsync();
            if (g is not null)
            {
                _game = g;
            }

            _ = SetPlayers();
        }
    }


    public void Dispose()
    {
        if (_matchService is null) { return; }
        _matchService.OnGameChange -= HandleGameStateChange;
    }
}