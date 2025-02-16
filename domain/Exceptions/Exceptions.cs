namespace disney_battle.domain.exceptions;

public abstract class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}


public class MissingConfigurationException(string config_key) : DomainException($"Missing {config_key} configuration")
{
}