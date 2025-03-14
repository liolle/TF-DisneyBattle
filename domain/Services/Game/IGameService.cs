
using disney_battle.cqs;
using disney_battle.dal.entities;

namespace disney_battle.domain.services;

public interface IGameService :
IQueryHandler<AllPersonages,ICollection<PersonageEntity>>
{

}
