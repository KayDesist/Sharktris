using SharkTetris.Models;

namespace SharkTetris.Services;

// OBSERVER — Concrete Subject
// Holds the list of observers and notifies all of them when an event fires.
// The game code that calls Publish() has no idea who's listening —
// it just sends the event and walks away.
public class GameEventService : IGameEventService
{
    // the subscriber list — ScoreObserver and GameLogObserver both live here
    private readonly List<IGameObserver> _observers = [];

    public void Subscribe(IGameObserver observer) => _observers.Add(observer);

    public void Unsubscribe(IGameObserver observer) => _observers.Remove(observer);

    // loops through every subscriber and tells them about the event
    public void Publish(GameEvent gameEvent)
    {
        foreach (var observer in _observers)
            observer.OnGameEvent(gameEvent); // each observer handles it in its own way
    }
}
