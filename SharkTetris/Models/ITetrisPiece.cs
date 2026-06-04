namespace SharkTetris.Models;

// FACTORY METHOD — Product Interface
// This defines what every Tetris piece must have.
// The factory returns this type, not TetrisPiece directly,
// so the game never depends on a specific class.
public interface ITetrisPiece
{
    int Type { get; }    // which piece (1=I, 2=O, 3=T, 4=S, 5=Z, 6=J, 7=L)
    int[][] Shape { get; } // 2D grid of 1s and 0s that draws the piece
    int StartX { get; }  // column where the piece spawns at the top
}
