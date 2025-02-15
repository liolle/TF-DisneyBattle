namespace disney_battle.cqs.queries;

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
    where TQuery : IQueryDefinition<TQuery>
    where TResult : IQueryDefinition<TResult>
{
    QueryResult<TResult> Execute(TQuery query);
}