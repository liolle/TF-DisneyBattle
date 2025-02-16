using disney_battle.dal.entities;

namespace disney_battle.domain.services;

public interface IJwtService
{
    public string generate(UserEntity user);
}