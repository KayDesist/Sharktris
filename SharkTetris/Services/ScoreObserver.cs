using SharkTetris.Models;

namespace SharkTetris.Services;

// OBSERVER — Concrete Observer #1
// Subscribes to game events and keeps track of score, lines, and level.
// It has no idea GameLogObserver exists — they're completely independent.
public class ScoreObserver : IGameObserver
{
    // classic Tetris scoring: more rows at once = way more points
    // 1 row = 100 pts, 2 rows = 300, 3 rows = 500, 4 rows (Tetris!) = 800
    private static readonly int[] PointsPerClear = [0, 100, 300, 500, 800];

    public int Score { get; private set; }
    public int LinesCleared { get; private set; }
    public int Level => LinesCleared / 10 + 1; // level up every 10 lines

    // this gets called by GameEventService.Publish() — this is the observer reacting
    public void OnGameEvent(GameEvent gameEvent)
    {
        switch (gameEvent.Type)
        {
            case GameEventType.RowsCleared:
                int rows = Math.Clamp(gameEvent.Value, 0, 4);
                LinesCleared += gameEvent.Value;
                Score += PointsPerClear[rows] * Level; // higher level = more points per clear
                break;

            case GameEventType.GameOver:
                // reset everything when the game ends
                Score = 0;
                LinesCleared = 0;
                break;
        }
    }
}
