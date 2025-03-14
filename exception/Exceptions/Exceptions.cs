namespace disney_battle.exceptions;

public abstract class DomainException : Exception
{
  public DomainException(string message) : base(message)
  {
  }
}

public class MissingConfigurationException(string config_key) : DomainException($"Missing {config_key} configuration")
{
}

public class MalformedInputException(string input) : DomainException($"Malformed input: {input} ")
{
}
