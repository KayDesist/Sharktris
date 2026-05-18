using SharkTetris.Models;
using SharkTetris.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────
builder.Services.AddRazorPages();

// Factory Pattern: piece creation is centralised behind an interface so the
// implementation (random, seeded, weighted) can be swapped without touching callers.
//
// Decorator Pattern: NextPiecePreviewFactory wraps TetrisPieceFactory to add
// a one-piece look-ahead queue. Callers that depend only on IPieceFactory are
// unaware of the decorator; the /api/piece/next endpoint uses the concrete type
// to expose the preview without changing any other part of the system.
builder.Services.AddSingleton<TetrisPieceFactory>();
// Register the concrete decorator so the /api/piece/next endpoint can inject
// it directly (to access NextPiece), then register it as the IPieceFactory
// implementation so the rest of the app uses it transparently.
builder.Services.AddSingleton<NextPiecePreviewFactory>(sp =>
    new NextPiecePreviewFactory(sp.GetRequiredService<TetrisPieceFactory>()));
builder.Services.AddSingleton<IPieceFactory>(sp =>
    sp.GetRequiredService<NextPiecePreviewFactory>());

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

// Decorator Pattern: returns the upcoming (preview) piece without consuming it
// from the queue. The client can use this to show a "next piece" preview panel.
// Only NextPiecePreviewFactory exposes the NextPiece property, so we resolve the
// concrete type here — the rest of the app still sees only IPieceFactory.
app.MapGet("/api/piece/next", (NextPiecePreviewFactory preview) =>
    Results.Ok(preview.NextPiece));

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
