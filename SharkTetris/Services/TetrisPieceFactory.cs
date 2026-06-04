using SharkTetris.Models;

namespace SharkTetris.Services;

// FACTORY METHOD — Concrete Creator
// This is THE factory. It's the only place in the whole app where
// pieces are actually built. If you wanted to change how pieces are
// made (e.g. seeded random, weighted bags), you'd only change this file.
public class TetrisPieceFactory : IPieceFactory
{
    private const int Cols = 10;

    // Each index is a piece type (1–7). Index 0 is unused so the
    // type number maps directly to the array position.
    // Each inner array is a row of 1s (filled) and 0s (empty).
    private static readonly int[][][] Shapes =
    [
        [],
        [[0,0,0,0],[1,1,1,1],[0,0,0,0],[0,0,0,0]],  // 1 — I
        [[1,1],[1,1]],                                   // 2 — O
        [[0,1,0],[1,1,1],[0,0,0]],                      // 3 — T
        [[0,1,1],[1,1,0],[0,0,0]],                      // 4 — S
        [[1,1,0],[0,1,1],[0,0,0]],                      // 5 — Z
        [[1,0,0],[1,1,1],[0,0,0]],                      // 6 — J
        [[0,0,1],[1,1,1],[0,0,0]],                      // 7 — L
    ];

    private readonly Random _random = new();

    // This is the factory method — picks a random type and delegates down
    public ITetrisPiece CreatePiece()
    {
        int type = _random.Next(1, 8); // random number 1 through 7
        return CreatePiece(type);
    }

    // Builds and returns the piece for a given type number
    public ITetrisPiece CreatePiece(int type)
    {
        if (type < 1 || type > 7)
            throw new ArgumentOutOfRangeException(nameof(type), "Piece type must be between 1 and 7.");

        var shape = Shapes[type];

        // center the piece horizontally on the board
        int startX = (int)Math.Floor(Cols / 2.0) - (int)Math.Floor(shape[0].Length / 2.0);

        // HERE is where the Concrete Product (TetrisPiece) gets created
        return new TetrisPiece
        {
            Type = type,
            Shape = shape.Select(row => row.ToArray()).ToArray(), // copy so rotations don't mutate the template
            StartX = startX,
        };
    }
}
