# SharkTetris

A shark-themed Tetris clone built with ASP.NET Core and JavaScript. This project is used as a sandbox for applying and practicing software design patterns throughout the course.

---

## Design Patterns Implemented

### 1. Factory Pattern

**Files:** `Services/IPieceFactory.cs`, `Services/TetrisPieceFactory.cs`, `Models/TetrisPiece.cs`

**Purpose:**  
Previously, Tetris piece generation was handled entirely inside the client-side JavaScript, tightly coupling game logic to the UI and making it untestable. Moving piece creation to a dedicated server-side factory centralizes the logic and decouples the client from it.

**Components in this codebase:**

- **Creator (Interface):** `IPieceFactory.cs` defines the contract for creating pieces (`CreatePiece()` and `CreatePiece(int type)`). Coding against the interface means the underlying implementation can be swapped — for example, with a seeded factory for replays or testing — without touching any calling code.
- **Concrete Creator:** `TetrisPieceFactory.cs` implements the interface. It holds all shape matrices for the seven standard Tetris pieces and contains the logic to center each piece horizontally at spawn.
- **Product:** `TetrisPiece.cs` is the model returned by the factory, carrying the piece's type, 2D shape matrix, and starting X coordinate.
- **Client:** The ASP.NET Core DI container injects `IPieceFactory` into the minimal API endpoint in `Program.cs`, which serves the generated piece to the frontend via `GET /api/piece`.

**Benefits:**
- The frontend no longer needs to know *how* a piece is built — it just asks for one.
- All piece logic is centralized in one maintainable class.
- New piece behaviors (weighted randomness, deterministic sequences) can be introduced by swapping the registered implementation with no other changes.

---

### 2. Observer Pattern

**Files:** `Services/IGameEventService.cs`, `Services/GameEventService.cs`, `Services/IGameObserver.cs`, `Services/ScoreObserver.cs`, `Services/GameLogObserver.cs`, `Models/GameEvent.cs`

**Purpose:**  
Game systems like scoring and logging need to react to events (rows cleared, game over, level up) without being hard-wired to each other. The Observer pattern allows event producers to stay completely unaware of their consumers.

**Components in this codebase:**

- **Subject (Interface):** `IGameEventService.cs` defines `Subscribe`, `Unsubscribe`, and `Publish` methods — the contract for the event hub.
- **Concrete Subject:** `GameEventService.cs` maintains a list of registered observers and fans out every published `GameEvent` to all of them.
- **Observer (Interface):** `IGameObserver.cs` defines a single `OnGameEvent(GameEvent)` method that all observers implement.
- **Concrete Observers:**
  - `ScoreObserver.cs` tracks score, lines cleared, and level using classic Tetris scoring rules (100 / 300 / 500 / 800 pts × level for 1 / 2 / 3 / 4 rows).
  - `GameLogObserver.cs` writes a log entry for every game event received.
- **Client:** The frontend reports events to the server via `POST /api/game/event`; the current score state is retrieved via `GET /api/game/score`.

**Benefits:**
- New reactions to game events (sound, animation triggers, achievements) can be added as new observers without changing `GameEventService` or any existing observer.
- Each observer owns its own state and logic, keeping classes small and single-purpose.

---

### 3. Decorator Pattern

**Files:** `Services/PieceFactoryDecorator.cs`, `Services/NextPiecePreviewFactory.cs`

**Purpose:**  
A "next piece" preview is a standard Tetris mechanic that helps players plan ahead. Rather than modifying `TetrisPieceFactory` directly (which would violate the Open/Closed Principle), the Decorator pattern wraps it to add the preview behavior without touching any existing code.

**Components in this codebase:**

- **Component Interface:** `IPieceFactory.cs` (shared with the Factory pattern) — defines the interface that both the wrapped factory and all decorators implement, allowing decorators to be used anywhere a factory is expected.
- **Concrete Component:** `TetrisPieceFactory.cs` — the existing factory being wrapped.
- **Decorator (Abstract Class):** `PieceFactoryDecorator.cs` holds a reference to the inner `IPieceFactory` and delegates both `CreatePiece` methods to it by default. Concrete decorators extend this and only override the methods they change, avoiding boilerplate repetition.
- **Concrete Decorator:** `NextPiecePreviewFactory.cs` wraps any `IPieceFactory` and maintains a one-piece look-ahead queue. Each call to `CreatePiece()` returns the pre-generated piece and immediately queues a fresh one as the new preview. A `NextPiece` property exposes the upcoming piece without consuming it from the queue.
- **Client:** `Program.cs` registers `NextPiecePreviewFactory` wrapping `TetrisPieceFactory` as the `IPieceFactory` singleton. All existing code that depends on `IPieceFactory` is unaware a decorator is in the chain. The new `GET /api/piece/next` endpoint injects `NextPiecePreviewFactory` directly to serve the preview piece to the frontend.

**Benefits:**
- A real game feature (next piece preview) was added without modifying any existing class.
- The decorator chain is transparent to all existing callers — nothing outside `Program.cs` had to change.
- Additional decorators (e.g., one that logs each piece spawn, or applies weighted randomness) can be stacked onto the chain independently.
