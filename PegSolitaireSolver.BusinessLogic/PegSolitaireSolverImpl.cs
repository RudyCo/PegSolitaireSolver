using PegSolitaireSolver.DataModel;

namespace PegSolitaireSolver.BusinessLogic;

public class PegSolitaireSolverImpl : IPegSolitaireSolver
{
    private List<Move> _solution = [];

    public SolverResult Solve(Board board)
    {
        _solution.Clear();
        DateTime startTime = DateTime.Now;

        bool solved = SolvePegSolitaire(board.Clone(), []);

        DateTime endTime = DateTime.Now;
        TimeSpan solutionTime = endTime - startTime;

        return new SolverResult
        {
            IsSolved = solved,
            Solution = solved ? [.. _solution] : [],
            SolutionTime = solutionTime
        };
    }

    private bool SolvePegSolitaire(Board board, List<Move> moves)
    {
        // Base case: if the board is solved
        if (board.IsSolved())
        {
            _solution = [.. moves];
            return true;
        }

        // If only one peg remains but not at center, this is not a valid solution
        if (board.CountPegs() == 1)
        {
            return false;
        }

        // Try all possible moves
        List<Move> possibleMoves = board.GetPossibleMoves();

        foreach (Move move in possibleMoves)
        {
            // Make the move
            board.ApplyMove(move);
            moves.Add(move);

            // Recursively try to solve
            if (SolvePegSolitaire(board, moves))
            {
                return true;
            }

            // Backtrack: undo the move
            board.UndoMove(move);
            moves.RemoveAt(moves.Count - 1);
        }

        return false;
    }
}