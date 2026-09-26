using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var postgres = builder.Configuration.GetConnectionString("Postgres");

builder.Services.AddDbContext<ScoreDb>(options =>
{
    if (string.IsNullOrWhiteSpace(postgres))
        options.UseSqlite(builder.Configuration.GetConnectionString("Scores"));
    else
        options.UseNpgsql(postgres);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ScoreDb>().Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/scores", async (CreateScoreRequest request, ScoreDb db) =>
{
    var playerName = request.PlayerName?.Trim().ToUpperInvariant() ?? string.Empty;

    var errors = new Dictionary<string, string[]>();

    if (playerName.Length != 3 || !playerName.All(char.IsAsciiLetter))
        errors["playerName"] = ["Tam 3 harf olmali."];

    if (request.Score is < 0 or > 100000)
        errors["score"] = ["0 ile 100000 arasinda olmali."];

    if (request.Kills is < 0 or > 999)
        errors["kills"] = ["0 ile 999 arasinda olmali."];

    if (request.Deaths is < 0 or > 999)
        errors["deaths"] = ["0 ile 999 arasinda olmali."];

    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    var entry = new ScoreEntry
    {
        PlayerName = playerName,
        Score = request.Score,
        Kills = request.Kills,
        Deaths = request.Deaths,
        CreatedAt = DateTimeOffset.UtcNow
    };

    db.Scores.Add(entry);
    await db.SaveChangesAsync();

    return Results.Created($"/scores/{entry.Id}", entry);
});

app.MapGet("/scores/top", async (ScoreDb db, int limit = 10) =>
{
    if (limit is < 1 or > 100)
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["limit"] = ["1 ile 100 arasinda olmali."]
        });

    var top = await db.Scores
        .OrderByDescending(s => s.Score)
        .ThenBy(s => s.Id)
        .Take(limit)
        .AsNoTracking()
        .ToListAsync();

    return Results.Ok(top);
});

app.Run();
