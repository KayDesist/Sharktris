using SharkTetris.Models;

namespace SharkTetris.Services;

// OBSERVER — Concrete Observer #2
// Also subscribes to game events, but its only job is logging.
// Adding this observer required zero changes to ScoreObserver or the game —
// that's the whole point of the Observer pattern.
public class GameLogObserver : IGameObserver
{
    private readonly ILogger<GameLogObserver> _logger;

    public GameLogObserver(ILogger<GameLogObserver> logger)
    {
        _logger = logger;
    }

    // called by GameEventService.Publish() — same entry point as ScoreObserver
    public void OnGameEvent(GameEvent gameEvent)
    {
        _logger.LogInformation("Game event: {EventType} value={Value}", gameEvent.Type, gameEvent.Value);
    }
}
