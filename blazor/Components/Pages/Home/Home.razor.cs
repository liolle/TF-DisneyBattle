using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace blazor.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
  bool Sending = false;

  int CurrentUserId;

  [Inject]
  private MatchService? MatchService { get; set; }

  [Inject]
  private AuthenticationStateProvider? AuthProvider { get; set; }

  [Inject]
  private NavigationManager? navigation {get;set;}


  protected override async Task OnInitializedAsync()
  {
    if (AuthProvider is null) { return; }
    var authState = await AuthProvider.GetAuthenticationStateAsync();
    var user = authState.User;

    if (!(user.Identity?.IsAuthenticated ?? false)) { return; }

    _ = int.TryParse(user.FindFirst("Id")?.Value, out CurrentUserId);

    if (MatchService is null) { return; }

    MatchService.JoinGame += HandleJoinGame;
  }

  private void HandleJoinGame(GameMatch gameMatch, Player player)
  {
    navigation?.NavigateTo("/game");
  }

  public async Task SearchGame()
  {
    if (MatchService is null || CurrentUserId == 0 || Sending) { return; }
    Sending = true;
    await MatchService.SearchGameAsync(CurrentUserId);
  }

  public void Dispose()
  {
    if (MatchService is null) { return; }
    MatchService.JoinGame -= HandleJoinGame;
  }

}
