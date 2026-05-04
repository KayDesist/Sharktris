using SharkTetris.Models;
using SharkTetris.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────
builder.Services.AddRazorPages();

// Factory Pattern: piece creation is centralised behind an interface so the
// implementation (random, seeded, weighted) can be swapped without touching callers.
builder.Services.AddSingleton<IPieceFactory, TetrisPieceFactory>();

// Observer Pattern: the event hub and all concrete observers are singletons so
// they accumulate state for the full lifetime of the application session.
builder.Services.AddSingleton<IGameEventService, GameEventService>();
builder.Services.AddSingleton<ScoreObserver>();
builder.Services.AddSingleton<GameLogObserver>();

var app = builder.Build();

// ── Wire up Observer subscriptions ────────────────────────────────────────
// Resolve the singleton event service and register each observer so they
// receive every game event published for the rest of the application's life.
var eventService = app.Services.GetRequiredService<IGameEventService>();
eventService.Subscribe(app.Services.GetRequiredService<ScoreObserver>());
eventService.Subscribe(app.Services.GetRequiredService<GameLogObserver>());

// ── Middleware pipeline ───────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// ── API endpoints ─────────────────────────────────────────────────────────

// Returns a randomly generated Tetris piece via the injected factory.
// The client calls this endpoint instead of generating pieces locally,
// keeping piece-creation logic server-side and testable.
app.MapGet("/api/piece", (IPieceFactory factory) =>
    Results.Ok(factory.CreatePiece()));

// Kept from the original project — serves the page title.
app.MapGet("/api/title", () =>
    Results.Ok(new { title = "🦈 Shark Tetris" }));

// Observer Pattern: the client reports game events to this endpoint.
// The event service fans the event out to every registered observer
// (ScoreObserver, GameLogObserver, and any added in future) without the
// endpoint needing to know anything about those consumers.
app.MapPost("/api/game/event", (GameEvent gameEvent, IGameEventService events) =>
{
    events.Publish(gameEvent);
    return Results.Ok();
});

// Returns the score state maintained by the ScoreObserver singleton.
app.MapGet("/api/game/score", (ScoreObserver score) =>
    Results.Ok(new { score.Score, score.LinesCleared, score.Level }));

app.Run();
