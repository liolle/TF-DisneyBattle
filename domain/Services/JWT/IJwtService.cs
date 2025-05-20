using System.Security.Claims;
using disney_battle.domain.services.models;

namespace disney_battle.domain.services;

public interface IJwtService
{
  public string Generate(List<Claim> claims);
}
