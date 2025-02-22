using disney_battle.cqs;
using disney_battle.domain.cqs.commands;
using disney_battle.domain.cqs.queries;
using disney_battle.domain.services.models;

namespace disney_battle.domain.services;


public interface IUserService :
    ICommandHandler<RegistersUserCommand>,
    IQueryHandler<UserFromUserNameQuery,CredentialInfoModel?>,
    IQueryHandler<CredentialLoginQuery,string>,
    IQueryHandlerAsync<OauthMicrosoftQuery,string>
{
    
}
