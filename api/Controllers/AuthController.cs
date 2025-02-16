using disney_battle.cqs;
using disney_battle.domain.cqs.commands;
using disney_battle.domain.cqs.queries;
using disney_battle.domain.services;
using disney_battle.exceptions;
using Microsoft.AspNetCore.Mvc;

public class AuthController(IUserService userService, IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    [Route("/register")]
    public IActionResult Register([FromBody] RegistersUserCommand command)
    {
        ICommandResult result = userService.Execute(command);
        if (result.IsFailure)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost]
    [Route("/login")]
    public IActionResult Login([FromBody] LoginQuery query)
    {

        try
        {
            string? token_name = configuration["AUTH_TOKEN_NAME"] ?? throw new MissingConfigurationException("AUTH_TOKEN_NAME");
            QueryResult<string> result = userService.Execute(query);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };
            Response.Cookies.Append(token_name, result.Result, cookieOptions);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest("Server error");
        }
    }

    [HttpGet]
    [Route("/logout")]
    public IActionResult Logout()
    {
        try
        {
            string? token_name = configuration["AUTH_TOKEN_NAME"] ?? throw new MissingConfigurationException("AUTH_TOKEN_NAME");
            Response.Cookies.Delete(token_name);
            return Ok(ICommandResult.Success());
        }
        catch (Exception e)
        {
            return BadRequest(ICommandResult.Failure(e.Message));
        }

    }
}

