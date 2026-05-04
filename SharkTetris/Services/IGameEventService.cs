using SharkTetris.Models;

namespace SharkTetris.Services;

/// <summary>
/// Subject interface — manages observer registration and event dispatch.
///
/// Design Pattern: Observer (Subject role)
/// </summary>
public interface IGameEventService
{
    void Subscribe(IGameObserver observer);
    void Unsubscribe(IGameObserver observer);
    void Publish(GameEvent gameEvent);
}
