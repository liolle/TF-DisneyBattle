namespace disney_battle.cqs.commands;

public interface ICommandDefinition
{
}

public interface ICommandHandler<T> where T : ICommandDefinition
{
}

public interface ICommandResult : IResult
{
    static CommandResult Success()
    {
        return new CommandResult(true, "", null);
    }

    static CommandResult Failure(string errorMessage, Exception? exception = null)
    {
        return new CommandResult(false, errorMessage, exception);
    }
}

public class CommandResult(bool isSuccess, string? errorMessage, Exception? exception) : ICommandResult
{
    public bool IsSuccess { get; } = isSuccess;

    public bool IsFailure { get; } = !isSuccess;

    public string? ErrorMessage { get; } = errorMessage;

    public Exception? Exception { get; } = exception;
}