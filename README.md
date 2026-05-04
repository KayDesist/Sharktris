# SharkTetris

# Design Pattern Implementation: Factory Pattern

## Purpose
For this milestone, I implemented the **Factory Pattern** to handle the creation of Tetris pieces in the game. Previously, piece generation was handled directly inside the client-side JavaScript, which tightly coupled the game logic to the UI layer and made it difficult to test or extend.

## How it Functions
By moving piece creation to a dedicated server-side factory, the client now requests pieces via a REST API endpoint (`/api/piece`). 

The pattern is broken down into the following components in my codebase:
* **Creator (Interface):** `IPieceFactory.cs` defines the contract for creating pieces. This allows the implementation to be easily swapped out later (e.g., for a seeded factory used in testing or replays) without changing the client code.
* **Concrete Creator:** `TetrisPieceFactory.cs` implements the interface, containing the actual logic and shape matrices required to generate a randomized `TetrisPiece`.
* **Product:** `TetrisPiece.cs` is the model representing the created object, containing the piece's type, 2D shape matrix, and starting coordinates.
* **Client:** The ASP.NET Core DI container injects the factory into the minimal API endpoint in `Program.cs`, which then serves the generated product to the frontend game loop in `index.html`.

## Benefits to the Project
1.  **Decoupling:** The frontend no longer needs to know *how* to build a Tetris piece; it just asks the factory for one. 
2.  **Maintainability:** All piece-generation logic, including centering and shape definitions, is centralized in one class.
3.  **Extensibility:** We can easily introduce new game modes (like a mode that weighs specific pieces heavier) by injecting a different `IPieceFactory` implementation without touching the frontend code.
