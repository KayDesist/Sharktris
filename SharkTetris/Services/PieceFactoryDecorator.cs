using SharkTetris.Models;

namespace SharkTetris.Services;

// DECORATOR — Abstract Base Decorator
// This is the "wrapper" layer. It holds a reference to any IPieceFactory
// and just forwards every call to it by default.
// Concrete decorators inherit from this and only override what they change.
// Key idea: it implements IPieceFactory AND holds an IPieceFactory —
// that's what makes stacking decorators possible.
public abstract class PieceFactoryDecorator : IPieceFactory
{
    // the factory being wrapped (could be TetrisPieceFactory or another decorator)
    protected readonly IPieceFactory _inner;

    protected PieceFactoryDecorator(IPieceFactory inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    // default behavior: just pass through to whatever is wrapped
    public virtual ITetrisPiece CreatePiece() => _inner.CreatePiece();
    public virtual ITetrisPiece CreatePiece(int type) => _inner.CreatePiece(type);
}
