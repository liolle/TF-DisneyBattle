namespace blazor.models;

public record Player(int id, string connectionId)
{
    public int PlayerId { get; set; } = id;
    public string ConId { get; set; } = connectionId;
}