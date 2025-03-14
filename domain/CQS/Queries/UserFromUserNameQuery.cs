using disney_battle.cqs;
using disney_battle.domain.services.models;

namespace disney_battle.domain.cqs.queries;


public class UserFromUserNameQuery(string userName) : IQueryDefinition<CredentialInfoModel?>
{
  public string UserName {get;} = userName;
}
