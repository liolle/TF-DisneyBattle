using disney_battle.cqs;

namespace disney_battle.domain.cqs.queries;


public class LoginQuery(string username, string password) : IQueryDefinition<string>
{
    public string UserName { get; } = username;
    public string Password { get; } = password;
}