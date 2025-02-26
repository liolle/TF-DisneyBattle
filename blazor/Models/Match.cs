namespace blazor.models;

public record GameMatch(int player1, int player2)
{
    public int P1 { get; set; } = player1;
    public int P2 { get; set; } = player2;

}