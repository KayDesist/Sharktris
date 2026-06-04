namespace SharkTetris.Models;

// FACTORY METHOD — Concrete Product
// The actual piece object the factory builds and returns.
// It implements ITetrisPiece, so callers only ever see the interface.
public class TetrisPiece : ITetrisPiece
{
    public int Type { get; set; }
    public int[][] Shape { get; set; } = Array.Empty<int[]>();
    public int StartX { get; set; }
}
