namespace PegSolitaireSolver.DataModel;

public class Board
{
    // Standard English Peg Solitaire board (33 holes)
    // 0 = empty hole, 1 = peg, -1 = invalid position
    private static readonly int[,] InitialBoardTemplate = {
        {-1, -1,  1,  1,  1, -1, -1},
        {-1, -1,  1,  1,  1, -1, -1},
        { 1,  1,  1,  1,  1,  1,  1},
        { 1,  1,  1,  0,  1,  1,  1}, // Center hole empty
        { 1,  1,  1,  1,  1,  1,  1},
        {-1, -1,  1,  1,  1, -1, -1},
        {-1, -1,  1,  1,  1, -1, -1}
    };

    private readonly int[,] _board;
    public const int BoardSize = 7;

    public Board()
    {
        _board = (int[,])InitialBoardTemplate.Clone();
    }

    public Board(int[,] boardState)
    {
        _board = (int[,])boardState.Clone();
    }

    public int this[int row, int col]
    {
        get => _board[row, col];
        set => _board[row, col] = value;
    }

    public Board Clone()
    {
        return new Board(_board);
    }

    public static bool IsValidPosition(int row, int col) => row >= 0 && row < BoardSize &&
               col >= 0 && col < BoardSize &&
               InitialBoardTemplate[row, col] != -1;

    public void ApplyMove(Move move)
    {
        // Remove peg from starting position
        _board[move.FromRow, move.FromCol] = 0;

        // Remove peg from middle position (jumped over)
        int middleRow = (move.FromRow + move.ToRow) / 2;
        int middleCol = (move.FromCol + move.ToCol) / 2;
        _board[middleRow, middleCol] = 0;

        // Place peg at target position
        _board[move.ToRow, move.ToCol] = 1;
    }

    public void UndoMove(Move move)
    {
        // Place peg back at starting position
        _board[move.FromRow, move.FromCol] = 1;

        // Place peg back at middle position
        int middleRow = (move.FromRow + move.ToRow) / 2;
        int middleCol = (move.FromCol + move.ToCol) / 2;
        _board[middleRow, middleCol] = 1;

        // Remove peg from target position
        _board[move.ToRow, move.ToCol] = 0;
    }

    public int CountPegs()
    {
        int count = 0;
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (_board[row, col] == 1)
                    count++;
            }
        }
        return count;
    }

    public List<Move> GetPossibleMoves()
    {
        List<Move> moves = [];

        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (_board[row, col] == 1) // If there's a peg here
                {
                    // Check all four directions (up, down, left, right)
                    int[,] directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

                    for (int d = 0; d < 4; d++)
                    {
                        int deltaRow = directions[d, 0];
                        int deltaCol = directions[d, 1];

                        int middleRow = row + deltaRow;
                        int middleCol = col + deltaCol;
                        int targetRow = row + 2 * deltaRow;
                        int targetCol = col + 2 * deltaCol;

                        // Check if move is valid
                        if (IsValidPosition(middleRow, middleCol) &&
                            IsValidPosition(targetRow, targetCol) &&
                            _board[middleRow, middleCol] == 1 && // Middle position has peg
                            _board[targetRow, targetCol] == 0)   // Target position is empty
                        {
                            moves.Add(new Move(row, col, targetRow, targetCol));
                        }
                    }
                }
            }
        }

        return moves;
    }

    public bool IsSolved()
    {
        return CountPegs() == 1 && _board[3, 3] == 1;
    }

    public void Display()
    {
        Console.WriteLine("    0 1 2 3 4 5 6");

        for (int row = 0; row < BoardSize; row++)
        {
            Console.Write($"{row}   ");
            for (int col = 0; col < BoardSize; col++)
            {
                switch (_board[row, col])
                {
                    case -1:
                        Console.Write("  ");
                        break;
                    case 0:
                        Console.Write("X ");
                        break;
                    case 1:
                        Console.Write("O ");
                        break;
                }
            }
            Console.WriteLine();
        }
    }

    public void DisplayWithNumbers()
    {
        Console.WriteLine("Current board state:");
        Console.WriteLine("O = peg, X = empty hole");
        Display();
    }
}