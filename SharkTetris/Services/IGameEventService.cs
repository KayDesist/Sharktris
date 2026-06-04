using SharkTetris.Models;

namespace SharkTetris.Services;

// OBSERVER — Subject Interface
// This is the "YouTube channel" side. It manages who is subscribed
// and broadcasts events to all of them via Publish().
public interface IGameEventService
{
    void Subscribe(IGameObserver observer);   // add a listener
    void Unsubscribe(IGameObserver observer); // remove a listener
    void Publish(GameEvent gameEvent);         // notify ALL current listeners
}
