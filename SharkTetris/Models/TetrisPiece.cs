namespace SharkTetris.Models;

/// <summary>
/// Represents a Tetris piece with its type, shape matrix, and starting column.
/// </summary>
public class TetrisPiece
{
    /// <summary>
    /// Piece type index (1–7), maps to a colour and shape on the client.
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 2-D shape matrix — rows of 0/1 values describing the piece cells.
    /// </summary>
    public int[][] Shape { get; set; } = Array.Empty<int[]>();

    /// <summary>
    /// Horizontal spawn column, pre-calculated so the piece spawns centred.
    /// </summary>
    public int StartX { get; set; }
}