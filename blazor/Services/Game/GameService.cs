using blazor.models;
using Microsoft.JSInterop;

namespace blazor.services;

public class GameService : IGameService
{
  private readonly IJSRuntime JS;


  public GameService(IJSRuntime jS)
  {
    JS = jS;
  }

  public async Task<List<Personage>> AllPersons()
  {
    ListResult<Personage> res = await JS.InvokeAsync<ListResult<Personage>>("allPersonage");
    return res.Result;
  }
}
