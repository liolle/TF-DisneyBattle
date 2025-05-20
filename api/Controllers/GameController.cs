using disney_battle.cqs;
using disney_battle.dal.entities;
using disney_battle.domain.services;
using edllx.dotnet.csrf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace disney_battle.api.controller;

public class GameController(IGameService gameService) : ControllerBase
{
  [HttpGet]
  [Route("/personage/all")]
  [EnableCors("auth-input")]

  [RequireCSRF]
  public IActionResult AllPersonage()
  {
    /*IQueryResult<ICollection<PersonageEntity>> result = gameService.Execute(new AllPersonages());
    if (result.IsFailure)
    {
      return BadRequest(result);
    }
    */
    List<PersonageEntity> l = [
      PersonageEntity.Create(0,"test",10,10,10)
    ];

    return Ok(l);
  }
}
