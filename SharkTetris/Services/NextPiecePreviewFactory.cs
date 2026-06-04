using SharkTetris.Models;

namespace SharkTetris.Services;

// DECORATOR — Concrete Decorator
// Wraps TetrisPieceFactory and adds a one-piece look-ahead.
// TetrisPieceFactory was NOT modified at all — new behavior was layered on top.
// This is the "add a lid to the coffee cup" moment.
public class NextPiecePreviewFactory : PieceFactoryDecorator
{
    private readonly object _lock = new();

    // the UI reads this to show the preview panel
    public ITetrisPiece NextPiece { get; private set; }

    public NextPiecePreviewFactory(IPieceFactory inner) : base(inner)
    {
        // pre-generate the first "next" piece so the preview is ready immediately
        NextPiece = _inner.CreatePiece();
    }

    // this is the added behavior — the original factory has nothing like this
    public override ITetrisPiece CreatePiece()
    {
        lock (_lock)
        {
            ITetrisPiece current = NextPiece;       // the queued piece becomes the current piece
            NextPiece = _inner.CreatePiece();        // pre-generate the next preview piece
            return current;                          // return what was queued
        }
    }

    // type-specific requests skip the queue and go straight to the inner factory
    // (CreatePiece(int type) inherits the pass-through from PieceFactoryDecorator)
}
