using disney_battle.cqs;
using disney_battle.dal.entities;

namespace disney_battle.domain.cqs.queries;


public class UserFromUserNameQuery(string userName) : IQueryDefinition<UserEntity?>
{
    public string UserName {get;} = userName;
}