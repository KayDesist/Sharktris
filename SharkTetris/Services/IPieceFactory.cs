using SharkTetris.Models;

namespace SharkTetris.Services;

/// <summary>
/// Factory interface for creating Tetris pieces.
/// Abstracting behind an interface keeps callers decoupled from the
/// concrete implementation and makes alternative factories easy to swap in
/// (e.g. a seeded/deterministic factory for replays or testing).
/// </summary>
public interface IPieceFactory
{
    /// <summary>Creates a randomly selected Tetris piece.</summary>
    TetrisPiece CreatePiece();

    /// <summary>Creates a specific Tetris piece by type (1–7).</summary>
    TetrisPiece CreatePiece(int type);
}