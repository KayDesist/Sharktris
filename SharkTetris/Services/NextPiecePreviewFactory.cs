using SharkTetris.Models;

namespace SharkTetris.Services;

/// <summary>
/// Concrete Decorator that adds a "next piece" preview to any
/// <see cref="IPieceFactory"/> without modifying the wrapped factory.
///
/// Design Pattern: Decorator
/// ─────────────────────────────────────────────────────────────────────────
/// Before this refactor, the client had no way to know which piece was coming
/// next — it could only request the current piece on demand. A "next piece"
/// preview is a standard Tetris feature that helps players plan ahead.
///
/// Rather than modifying <see cref="TetrisPieceFactory"/> (breaking the
/// Open/Closed Principle), we wrap it in this decorator. The decorator:
///   1. Pre-generates one piece at construction time and stores it as
///      <see cref="NextPiece"/> (the upcoming piece the client can preview).
///   2. On every <see cref="CreatePiece()"/> call it returns the stored
///      preview piece as the "current" piece, then immediately generates a
///      fresh piece to become the new preview — maintaining a one-piece
///      look-ahead at all times.
///   3. Delegates <see cref="CreatePiece(int)"/> (used for seeded/test
///      requests) straight through to the inner factory unchanged.
///
/// The result: callers still use only <see cref="IPieceFactory"/> — they are
/// completely unaware that a decorator is in the chain — while a dedicated
/// endpoint can expose <see cref="NextPiece"/> for the preview UI.
/// </summary>
public class NextPiecePreviewFactory : PieceFactoryDecorator
{
    private readonly object _lock = new();

    /// <summary>
    /// The piece that will be returned by the next <see cref="CreatePiece()"/>
    /// call. Clients can read this via <c>GET /api/piece/next</c> to render a
    /// preview without consuming the piece from the queue.
    /// </summary>
    public TetrisPiece NextPiece { get; private set; }

    /// <summary>
    /// Wraps <paramref name="inner"/> and pre-generates the first preview piece.
    /// </summary>
    public NextPiecePreviewFactory(IPieceFactory inner) : base(inner)
    {
        // Seed the look-ahead queue with one piece so NextPiece is never null.
        NextPiece = _inner.CreatePiece();
    }

    /// <summary>
    /// Returns the pre-generated preview piece as the "current" piece and
    /// immediately queues a fresh piece as the new preview.
    /// </summary>
    public override TetrisPiece CreatePiece()
    {
        lock (_lock)
        {
            TetrisPiece current = NextPiece;
            NextPiece = _inner.CreatePiece(); // advance the look-ahead
            return current;
        }
    }

    // CreatePiece(int type) is intentionally not overridden — the base
    // decorator delegates it straight to the inner factory, bypassing the
    // preview queue (correct: type-specific requests are used for testing
    // and replays, not normal gameplay flow).
}
