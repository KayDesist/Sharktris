using SharkTetris.Models;

namespace SharkTetris.Services;

/// <summary>
/// Observer interface — any component that needs to react to game events
/// implements this interface and registers with <see cref="IGameEventService"/>.
///
/// Design Pattern: Observer (Observer role)
/// </summary>
public interface IGameObserver
{
    void OnGameEvent(GameEvent gameEvent);
}
