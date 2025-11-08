using PegSolitaireSolver.BusinessLogic;
using PegSolitaireSolver.DataModel;

Console.WriteLine("Peg Solitaire Solver using Backtracking");
Console.WriteLine("========================================");
Console.WriteLine();

// Create a new board
Board board = new();

Console.WriteLine("Initial Board:");
board.Display();
Console.WriteLine();

int initialPegs = board.CountPegs();
Console.WriteLine($"Starting with {initialPegs} pegs");
Console.WriteLine("Goal: Remove all pegs except one, which must end up at the center");
Console.WriteLine("Solving... (this may take a moment)");
Console.WriteLine();

// Use the business logic solver
PegSolitaireSolverImpl solver = new();
SolverResult result = solver.Solve(board);

if (result.IsSolved)
{
    Console.WriteLine($"Solution found in {result.SolutionTime.TotalSeconds:F2} seconds!");
    Console.WriteLine($"Solution requires {result.MoveCount} moves:");
    Console.WriteLine();

    // Reset board to show solution step by step
    board = new Board();

    Console.WriteLine("Step-by-step solution:");
    Console.WriteLine("Press Enter to see each move...");
    Console.ReadLine();

    board.DisplayWithNumbers();
    Console.WriteLine();

    for (int i = 0; i < result.Solution.Count; i++)
    {
        Move move = result.Solution[i];
        board.ApplyMove(move);

        Console.WriteLine($"Move {i + 1}: {move}");
        board.DisplayWithNumbers();
        Console.WriteLine($"Pegs remaining: {board.CountPegs()}");
        Console.WriteLine();

        if (i < result.Solution.Count - 1)
        {
            Console.WriteLine("Press Enter for next move...");
            Console.ReadLine();
        }
    }

    Console.WriteLine("*** Puzzle solved! Only one peg remains at the center of the board.");
}
else
{
    Console.WriteLine("XXX No solution found.");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
