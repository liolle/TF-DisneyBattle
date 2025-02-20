namespace blazor.models;

public record GameMatch(Player player1, Player player2)
{
    public Player Player1 { get; set; } = player1;
    public Player Player2 { get; set; } = player2;

    public bool UpdatePlayerConnection(Dictionary<int, Player> player_poll){
        
        if (!ValidateKeysExist(player_poll)){return false;}

        player1.ConnectionId = player_poll[player1.Id].ConnectionId;
        player2.ConnectionId = player_poll[player2.Id].ConnectionId;
        
        return true;
    }

    public bool ValidateKeysExist(Dictionary<int, Player> player_poll){
        return player_poll.ContainsKey(Player1.Id) && player_poll.ContainsKey(Player2.Id);
    }

}