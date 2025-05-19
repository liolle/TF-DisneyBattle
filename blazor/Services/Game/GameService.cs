using blazor.models;
using Microsoft.JSInterop;

namespace blazor.services;

public class GameService : IGameService
{
  private readonly IJSRuntime JS;
  private readonly HttpInfoService _info;


  public GameService(IJSRuntime jS, HttpInfoService infoService)
  {
    JS = jS;
    _info = infoService;
  }

  public async Task<List<Personage>> AllPersons()
  {
    var res = await JS.InvokeAsync<dynamic>("allPersonage", _info.CSRF_CODE);
    return [];
  }
}
