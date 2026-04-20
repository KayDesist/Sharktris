using SharkTetris.Models;

namespace SharkTetris.Services;

/// <summary>
/// Concrete factory that produces <see cref="TetrisPiece"/> instances.
///
/// Design Pattern: Factory
/// ─────────────────────────────────────────────────────────────────────────
/// Before this refactor, piece creation was handled by a bare
/// <c>randomPiece()</c> JavaScript function — tightly coupled to the client,
/// untestable, and impossible to extend without touching the UI layer.
///
/// Moving piece creation into a dedicated server-side factory:
///   • Centralises piece logic in one maintainable class.
///   • Exposes a stable interface (<see cref="IPieceFactory"/>) so the
///     implementation (random, seeded, weighted, etc.) can change without
///     touching callers.
///   • Enables dependency injection and unit testing of piece generation.
///   • Opens the door to server-authoritative game modes or replays.
/// </summary>
public class TetrisPieceFactory : IPieceFactory
{
    private const int Cols = 10;

    // Each index matches the piece type used by the client (1-indexed).
    // Index 0 is a placeholder so type numbers map directly.
    private static readonly int[][][] Shapes =
    [
        [],                                                   // 0 — unused
        [[0,0,0,0],[1,1,1,1],[0,0,0,0],[0,0,0,0]],          // 1 — I
        [[1,1],[1,1]],                                         // 2 — O
        [[0,1,0],[1,1,1],[0,0,0]],                            // 3 — T
        [[0,1,1],[1,1,0],[0,0,0]],                            // 4 — S
        [[1,1,0],[0,1,1],[0,0,0]],                            // 5 — Z
        [[1,0,0],[1,1,1],[0,0,0]],                            // 6 — J
        [[0,0,1],[1,1,1],[0,0,0]],                            // 7 — L
    ];

    private readonly Random _random = new();

    /// <inheritdoc/>
    public TetrisPiece CreatePiece()
    {
        int type = _random.Next(1, 8); // 1 inclusive, 8 exclusive → types 1–7
        return CreatePiece(type);
    }

    /// <inheritdoc/>
    public TetrisPiece CreatePiece(int type)
    {
        if (type < 1 || type > 7)
            throw new ArgumentOutOfRangeException(nameof(type), "Piece type must be between 1 and 7.");

        var shape = Shapes[type];

        // Centre the piece horizontally at spawn — mirrors the original JS logic.
        int startX = (int)Math.Floor(Cols / 2.0) - (int)Math.Floor(shape[0].Length / 2.0);

        return new TetrisPiece
        {
            Type = type,
            Shape = shape.Select(row => row.ToArray()).ToArray(), // deep copy
            StartX = startX,
        };
    }
}