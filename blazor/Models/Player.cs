namespace blazor.models;

public record Player(int id, string connectionId)
{
    public int Id { get; set; } = id;
    public string ConnectionId { get; set; } = connectionId;
}