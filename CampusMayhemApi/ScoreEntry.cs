public class ScoreEntry
{
    public int Id { get; set; }
    public required string PlayerName { get; set; }
    public int Score { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public record CreateScoreRequest(string PlayerName, int Score, int Kills, int Deaths);
