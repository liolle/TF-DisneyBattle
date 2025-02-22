namespace disney_battle.cqs;

public interface IQueryDefinition<TResult>
{
}

public class QueryResult<TResult>(bool isSuccess,TResult result, string errorMessage, Exception? exception) : IQueryResult<TResult>
{
    public bool IsSuccess { get; } = isSuccess;

    public bool IsFailure { get; } = !isSuccess;

    public string? ErrorMessage { get; } = errorMessage;

    public TResult Result {
        get {
            if (IsFailure){throw new InvalidOperationException();}
            return result;
        }
    }

    public Exception? Exception { get; } = exception;
}

public interface IQueryResult<TResult> : IResult
{
    static QueryResult<TResult> Success(TResult result){
        return new QueryResult<TResult>(true,result,"",null);
    }

    static QueryResult<TResult> Failure(string errorMessage, Exception? exception = null){
        return new QueryResult<TResult>(false,default!,errorMessage,exception);
    }
}

public interface IQueryHandler<TQuery,TResult> 
    : IQueryDefinition<TResult> where TQuery : IQueryDefinition<TResult>
{
    QueryResult<TResult> Execute(TQuery query);
}


public interface IQueryHandlerAsync<TQuery,TResult> 
    : IQueryDefinition<TResult> where TQuery : IQueryDefinition<Task<TResult>>
{
    Task<QueryResult<TResult>> Execute(TQuery query);
}