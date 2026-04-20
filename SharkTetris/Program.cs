using SharkTetris.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────
builder.Services.AddRazorPages();

// Register the piece factory with the DI container.
// Singleton is appropriate here: TetrisPieceFactory holds no per-request
// state and Random is safe to reuse across calls within a single instance.
builder.Services.AddSingleton<IPieceFactory, TetrisPieceFactory>();

var app = builder.Build();

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

app.Run();