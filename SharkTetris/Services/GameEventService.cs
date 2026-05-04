using SharkTetris.Models;

namespace SharkTetris.Services;

/// <summary>
/// Concrete Subject in the Observer pattern.
///
/// Design Pattern: Observer
/// ─────────────────────────────────────────────────────────────────────────
/// GameEventService is the central event hub. Any game component that cares
/// about game events (scoring, logging, future sound or animation systems)
/// registers itself as an <see cref="IGameObserver"/>. When the client reports
/// an event — rows cleared, game over, etc. — this class forwards it to every
/// registered observer without knowing or caring what each one does.
///
/// This decoupling means:
///   • New observers can be added without changing this class or any other
///     existing observer.
///   • Event producers (API endpoints) only depend on IGameEventService,
///     not on any specific downstream consumer.
///   • Each observer owns its own state and logic, keeping classes small and
///     single-purpose.
/// </summary>
public class GameEventService : IGameEventService
{
    private readonly List<IGameObserver> _observers = [];

    public void Subscribe(IGameObserver observer) => _observers.Add(observer);

    public void Unsubscribe(IGameObserver observer) => _observers.Remove(observer);

    public void Publish(GameEvent gameEvent)
    {
        foreach (var observer in _observers)
            observer.OnGameEvent(gameEvent);
    }
}
