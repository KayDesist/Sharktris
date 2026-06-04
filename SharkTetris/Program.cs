using SharkTetris.Models;
using SharkTetris.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Factory Method + Decorator wiring:
// TetrisPieceFactory is the base concrete creator.
// NextPiecePreviewFactory wraps it to add next-piece look-ahead.
// The rest of the app only ever sees IPieceFactory.
builder.Services.AddSingleton<TetrisPieceFactory>();
builder.Services.AddSingleton<NextPiecePreviewFactory>(sp =>
    new NextPiecePreviewFactory(sp.GetRequiredService<TetrisPieceFactory>()));
builder.Services.AddSingleton<IPieceFactory>(sp =>
    sp.GetRequiredService<NextPiecePreviewFactory>());

// Observer wiring: event hub + observers registered as singletons so they
// accumulate state for the whole application session.
builder.Services.AddSingleton<IGameEventService, GameEventService>();
builder.Services.AddSingleton<ScoreObserver>();
builder.Services.AddSingleton<GameLogObserver>();

var app = builder.Build();

// Subscribe both observers so they receive every event from here on.
var eventService = app.Services.GetRequiredService<IGameEventService>();
eventService.Subscribe(app.Services.GetRequiredService<ScoreObserver>());
eventService.Subscribe(app.Services.GetRequiredService<GameLogObserver>());

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

// Factory Method: creates a new piece through the interface.
app.MapGet("/api/piece", (IPieceFactory factory) =>
    Results.Ok(factory.CreatePiece()));

// Decorator: exposes the upcoming preview piece without consuming it.
app.MapGet("/api/piece/next", (NextPiecePreviewFactory preview) =>
    Results.Ok(preview.NextPiece));

app.MapGet("/api/title", () =>
    Results.Ok(new { title = "🦈 Shark Tetris" }));

// Observer: client reports events here; the hub notifies all observers.
app.MapPost("/api/game/event", (GameEvent gameEvent, IGameEventService events) =>
{
    events.Publish(gameEvent);
    return Results.Ok();
});

// Returns score state from the ScoreObserver singleton.
app.MapGet("/api/game/score", (ScoreObserver score) =>
    Results.Ok(new { score.Score, score.LinesCleared, score.Level }));

app.Run();
