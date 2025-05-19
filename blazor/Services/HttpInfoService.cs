using System.Text.RegularExpressions;
using edllx.dotnet.csrf;

namespace blazor.services;

public class HttpInfoService
{
    public string ACCESS_TOKEN { get; init; }
    public string CSRF_COOKIE { get; init; }
    public string CSRF_CODE { get; init; }

    int r = new Random().Next();

    public int RANDOM { get { return r; } }

    public HttpInfoService(IHttpContextAccessor httpContextAccessor, IConfiguration config)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext ?? throw new Exception("Missing dependency HttpContextAccessor");
        string COOKIE_NAME = config["AUTH_TOKEN_NAME"] ?? throw new Exception("Missing configuration: AUTH_TOKEN_NAME");
        string CSRF_COOKIE_NAME = config["CSRF_COOKIE_NAME"] ?? throw new Exception("Missing configuration: CSRF_COOKIE_NAME");
        string CSRF_HEADER_NAME = config["CSRF_HEADER_NAME"] ?? throw new Exception("Missing configuration: CSRF_HEADER_NAME");

        ACCESS_TOKEN = httpContext.Request.Cookies[COOKIE_NAME] ?? "";
        CSRF_COOKIE = httpContext.Request.Cookies[CSRF_COOKIE_NAME] ?? "";
        CSRF_CODE = (httpContext.Items[CSRF_HEADER_NAME] as string) ?? "";
    }
}