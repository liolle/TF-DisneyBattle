using blazor.models;

namespace blazor.services;

public interface IGameService
{
  Task<List<Personage>> AllPersons();
}
