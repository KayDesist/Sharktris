namespace SharkTetris.Models;

/// <summary>Types of events the game client can report to the server.</summary>
public enum GameEventType
{
    RowsCleared,
    GameOver,
    PieceSpawned,
    LevelUp
}

/// <summary>
/// Represents a single game event published by the client.
/// <see cref="Value"/> carries contextual data — for <see cref="GameEventType.RowsCleared"/>
/// it holds the number of rows cleared simultaneously; otherwise it is unused.
/// </summary>
public class GameEvent
{
    public GameEventType Type { get; set; }
    public int Value { get; set; }
}
