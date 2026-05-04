using SharkTetris.Models;

namespace SharkTetris.Services;

/// <summary>
/// Concrete Observer — tracks score, lines cleared, and level in response to
/// game events published through <see cref="IGameEventService"/>.
///
/// Design Pattern: Observer (Concrete Observer role)
///
/// Scoring follows classic Tetris rules (points multiplied by current level):
///   1 row  →  100 pts  |  2 rows → 300 pts
///   3 rows →  500 pts  |  4 rows → 800 pts
/// Level increases by 1 for every 10 lines cleared.
/// </summary>
public class ScoreObserver : IGameObserver
{
    private static readonly int[] PointsPerClear = [0, 100, 300, 500, 800];

    public int Score { get; private set; }
    public int LinesCleared { get; private set; }
    public int Level => LinesCleared / 10 + 1;

    public void OnGameEvent(GameEvent gameEvent)
    {
        switch (gameEvent.Type)
        {
            case GameEventType.RowsCleared:
                int rows = Math.Clamp(gameEvent.Value, 0, 4);
                LinesCleared += gameEvent.Value;
                Score += PointsPerClear[rows] * Level;
                break;

            case GameEventType.GameOver:
                Score = 0;
                LinesCleared = 0;
                break;
        }
    }
}
