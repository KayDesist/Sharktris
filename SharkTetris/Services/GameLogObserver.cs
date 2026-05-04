using SharkTetris.Models;

namespace SharkTetris.Services;

/// <summary>
/// Concrete Observer — writes a log entry for every game event received.
///
/// Design Pattern: Observer (Concrete Observer role)
/// </summary>
public class GameLogObserver : IGameObserver
{
    private readonly ILogger<GameLogObserver> _logger;

    public GameLogObserver(ILogger<GameLogObserver> logger)
    {
        _logger = logger;
    }

    public void OnGameEvent(GameEvent gameEvent)
    {
        _logger.LogInformation("Game event: {EventType} value={Value}", gameEvent.Type, gameEvent.Value);
    }
}
