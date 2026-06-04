using SharkTetris.Models;

namespace SharkTetris.Services;

// OBSERVER — Observer Interface
// Every class that wants to react to game events implements this.
// When the subject (GameEventService) calls Publish(), it calls
// OnGameEvent() on every subscriber — this method is the entry point.
public interface IGameObserver
{
    void OnGameEvent(GameEvent gameEvent);
}
