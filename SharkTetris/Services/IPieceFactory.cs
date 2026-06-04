using SharkTetris.Models;

namespace SharkTetris.Services;

// FACTORY METHOD — Creator Interface
// This is the factory contract. Anyone who wants to make pieces
// must implement these two methods.
// The game only ever talks to this interface — never to TetrisPieceFactory directly.
public interface IPieceFactory
{
    ITetrisPiece CreatePiece();        // make a random piece
    ITetrisPiece CreatePiece(int type); // make a specific piece by type number
}
