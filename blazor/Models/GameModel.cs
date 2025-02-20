namespace blazor.models;

public class ListResult<T>
{
    public bool IsSuccess { get; set; }

    public bool IsFailure { get; set; }

    public string? ErrorMessage { get; set; }

    public List<T> Result { get; set; } = [];

    public string? Exception { get; }
}