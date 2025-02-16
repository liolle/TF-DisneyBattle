using disney_battle.cqs;
using disney_battle.dal.entities;
using disney_battle.domain.cqs.commands;
using disney_battle.domain.cqs.queries;

namespace disney_battle.domain.services;


public interface IUserService :
    ICommandHandler<RegistersUserCommand>,
    IQueryHandler<UserFromUserNameQuery,UserEntity?>,
    IQueryHandler<LoginQuery,string>
{
    
}