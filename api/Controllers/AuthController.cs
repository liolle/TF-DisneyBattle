using disney_battle.cqs;
using disney_battle.domain.cqs.commands;
using disney_battle.domain.cqs.queries;
using disney_battle.domain.services;
using disney_battle.exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace disney_battle.api.controller;

public class AuthController(IUserService userService, IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    [Route("/register")]
    [EnableCors("auth-input")]
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
    [EnableCors("auth-input")]
    public IActionResult Login([FromBody] CredentialLoginQuery query)
    {

        try
        {
            string? token_name = configuration["AUTH_TOKEN_NAME"] ?? throw new MissingConfigurationException("AUTH_TOKEN_NAME");
            QueryResult<string> result = userService.Execute(query);
            if (result.IsFailure)
            {
                if (query.Redirect_Failure_Uri is not null)
                {
                    Redirect(query.Redirect_Failure_Uri);
                }
                return BadRequest(result.ErrorMessage);
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };
            Response.Cookies.Append(token_name, result.Result, cookieOptions);
            if (query.Redirect_Success_Uri is not null)
            {
                Redirect(query.Redirect_Success_Uri);
            }
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest("Server error");
        }
    }

    [HttpGet]
    [Route("/logout")]
    [EnableCors("auth-input")]
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

    [HttpGet]
    [Route("/auth")]
    [EnableCors("auth-input")]
    [Authorize]
    public IActionResult Auth()
    {
        int.TryParse(User.FindFirst("id")?.Value, out int id);
        var result = new { id = id, email = User.FindFirst("email")?.Value };

        return Ok(result);
    }

    [HttpPost]
    [Route("/oauth/microsoft")]
    public async Task<IActionResult> OauthMicrosoft([FromBody] OauthMicrosoftQuery query)
    {
        QueryResult<string> result = await userService.Execute(query);
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(60)
        };
        if (result.IsFailure)
        {
            Response.Cookies.Append("test", "Failed", cookieOptions);
            return Redirect(query.Redirect_Failure_Uri);
        }


        Response.Cookies.Append("test", result.Result, cookieOptions);

        return Redirect($"{query.Redirect_Success_Uri}");
    }

    [HttpPost]
    [Route("/oauth/google")]
    public IActionResult OauthGoogle()
    {
        return Redirect("https://localhot:7145");
    }
}

