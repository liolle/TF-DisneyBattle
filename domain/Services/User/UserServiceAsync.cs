using System.Net.Http.Json;
using disney_battle.cqs;
using disney_battle.domain.cqs.queries;
using disney_battle.domain.services.models;
using disney_battle.exceptions;

namespace disney_battle.domain.services;

public partial class UserService
{
    public async Task<QueryResult<string>> Execute(OauthMicrosoftQuery query)
    {

        try
        {
            string client_id = configuration["CLIENT_ID"] ?? throw new MissingConfigurationException("CLIENT_ID");
            string client_secret = configuration["CLIENT_SECRET"] ?? throw new MissingConfigurationException("CLIENT_SECRET");
            string tenant_id = configuration["TENANT_ID"] ?? throw new MissingConfigurationException("TENANT_ID");

            var tokenResponse = await httpClient.PostAsync(
                $"https://login.microsoftonline.com/{tenant_id}/oauth2/v2.0/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id",  client_id},
                    { "client_secret", client_secret },
                    { "code", query.Code },
                    { "redirect_uri", query.Redirect_Success_Uri },
                    { "scope", "openid profile email" },
                    { "grant_type", "authorization_code" }
                })
            );

            var res = await tokenResponse.Content.ReadFromJsonAsync<MicrosoftTokenModel>();

            if (res is null)
            {
                return IQueryResult<string>.Failure("Authentication failed");
            }

            return IQueryResult<string>.Success(res.Access_Token);
        }
        catch (Exception)
        {
            return IQueryResult<string>.Failure("Server error");
        }
    }
}