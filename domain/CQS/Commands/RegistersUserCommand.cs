using disney_battle.cqs;

namespace disney_battle.domain.cqs.commands;

public class RegistersUserCommand(string userName, string email, string password) : ICommandDefinition
{
    public string UserName { get; set; } = userName;
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
}