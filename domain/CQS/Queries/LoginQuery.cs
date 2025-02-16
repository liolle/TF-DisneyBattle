using disney_battle.cqs;

namespace disney_battle.domain.cqs.queries;


public class LoginQuery(string userName, string password) : IQueryDefinition<string>
{
    public string UserName { get; } = userName;
    public string Password { get; } = password;
}