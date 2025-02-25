namespace blazor.models;

public record GameMatch(Player player1, Player player2)
{
    public Player P1 { get; set; } = player1;
    public Player P2 { get; set; } = player2;

    public bool UpdatePlayerConnection(Dictionary<int, Player> player_poll){
        
        if (!ValidateKeysExist(player_poll)){return false;}

        player1.ConId = player_poll[player1.PlayerId].ConId;
        player2.ConId = player_poll[player2.PlayerId].ConId;
        
        return true;
    }

    public bool ValidateKeysExist(Dictionary<int, Player> player_poll){
        return player_poll.ContainsKey(P1.PlayerId) && player_poll.ContainsKey(P2.PlayerId);
    }

}