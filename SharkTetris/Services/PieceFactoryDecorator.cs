using SharkTetris.Models;

namespace SharkTetris.Services;

/// <summary>
/// Abstract base decorator for <see cref="IPieceFactory"/>.
///
/// Design Pattern: Decorator
/// ─────────────────────────────────────────────────────────────────────────
/// A Decorator wraps an existing object (the "component") and implements the
/// same interface so it can be used anywhere the original would be used.
/// Concrete decorators then override only the methods they want to extend,
/// delegating everything else to the wrapped inner factory.
///
/// This base class handles the delegation boilerplate so each concrete
/// decorator only needs to override the methods it actually changes.
/// </summary>
public abstract class PieceFactoryDecorator : IPieceFactory
{
    /// <summary>The wrapped factory that this decorator delegates to.</summary>
    protected readonly IPieceFactory _inner;

    protected PieceFactoryDecorator(IPieceFactory inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    /// <inheritdoc/>
    public virtual TetrisPiece CreatePiece() => _inner.CreatePiece();

    /// <inheritdoc/>
    public virtual TetrisPiece CreatePiece(int type) => _inner.CreatePiece(type);
}
