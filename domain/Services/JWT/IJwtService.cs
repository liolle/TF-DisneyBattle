using disney_battle.domain.services.models;

namespace disney_battle.domain.services;

public interface IJwtService
{
    public string generate(CredentialInfoModel user);
}