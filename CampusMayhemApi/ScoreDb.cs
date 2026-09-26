using Microsoft.EntityFrameworkCore;

public class ScoreDb(DbContextOptions<ScoreDb> options) : DbContext(options)
{
    public DbSet<ScoreEntry> Scores => Set<ScoreEntry>();
}
