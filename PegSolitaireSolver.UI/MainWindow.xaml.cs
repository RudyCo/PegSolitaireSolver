using PegSolitaireSolver.BusinessLogic;
using PegSolitaireSolver.DataModel;
using System.Windows;
using System.Windows.Controls;

namespace ConsoleAppPrompt.UI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Board _gameBoard;
    private readonly PegSolitaireSolverImpl _solver;
    private SolverResult? _currentSolution;
    private int _currentMoveIndex;
    private readonly Button[,] _boardButtons;

    public MainWindow()
    {
        InitializeComponent();
        _gameBoard = new Board();
        _solver = new PegSolitaireSolverImpl();
        _boardButtons = new Button[Board.BoardSize, Board.BoardSize];
        _currentMoveIndex = 0;

        InitializeBoard();
        UpdateDisplay();
    }

    private void InitializeBoard()
    {
        BoardGrid.Children.Clear();

        for (int row = 0; row < Board.BoardSize; row++)
        {
            for (int col = 0; col < Board.BoardSize; col++)
            {
                Button button = new()
                {
                    Style = (Style)Resources["PegButtonStyle"],
                    Tag = new { Row = row, Col = col }
                };

                _boardButtons[row, col] = button;
                BoardGrid.Children.Add(button);
            }
        }
    }

    private void UpdateDisplay()
    {
        // Update board buttons
        for (int row = 0; row < Board.BoardSize; row++)
        {
            for (int col = 0; col < Board.BoardSize; col++)
            {
                Button button = _boardButtons[row, col];
                int cellValue = _gameBoard[row, col];

                switch (cellValue)
                {
                    case -1: // Invalid position
                        button.Content = "";
                        button.IsEnabled = false;
                        break;
                    case 0: // Empty hole
                        button.Content = "○";
                        button.IsEnabled = true;
                        break;
                    case 1: // Peg
                        button.Content = "●";
                        button.IsEnabled = true;
                        break;
                }
            }
        }

        // Update statistics
        PegsRemainingText.Text = $"Pegs Remaining: {_gameBoard.CountPegs()}";
        MovesPlayedText.Text = $"Moves Played: {_currentMoveIndex}";

        // Update solution status
        if (_currentSolution != null)
        {
            if (_currentMoveIndex >= _currentSolution.Solution.Count)
            {
                SolutionStatusText.Text = _gameBoard.IsSolved() ? "Puzzle Solved!" : "Solution Complete";
                SolutionProgressBar.Value = 100;
            }
            else
            {
                SolutionStatusText.Text = $"Solution loaded ({_currentSolution.Solution.Count} moves)";
                double progress = (_currentMoveIndex / (double)_currentSolution.Solution.Count) * 100;
                SolutionProgressBar.Value = progress;
            }
        }
        else
        {
            SolutionStatusText.Text = "No solution loaded";
            SolutionProgressBar.Value = 0;
        }
    }

    private async void SolveButton_Click(object sender, RoutedEventArgs e)
    {
        SolveButton.IsEnabled = false;
        SolutionStatusText.Text = "Solving... Please wait";

        try
        {
            // Run solver in background to keep UI responsive
            _currentSolution = await Task.Run(() => _solver.Solve(_gameBoard));

            if (_currentSolution.IsSolved)
            {
                SolutionTimeText.Text = $"Solution Time: {_currentSolution.SolutionTime.TotalSeconds:F2}s";

                // Populate moves list
                MovesListBox.Items.Clear();
                for (int i = 0; i < _currentSolution.Solution.Count; i++)
                {
                    Move move = _currentSolution.Solution[i];
                    MovesListBox.Items.Add($"{i + 1:D2}: {move}");
                }

                NextMoveButton.IsEnabled = true;
                PlayAllButton.IsEnabled = true;

                MessageBox.Show($"Solution found with {_currentSolution.Solution.Count} moves in {_currentSolution.SolutionTime.TotalSeconds:F2} seconds!",
                               "Solution Found", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No solution found for the current board state.",
                               "No Solution", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error solving puzzle: {ex.Message}",
                           "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            SolveButton.IsEnabled = true;
            UpdateDisplay();
        }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        _gameBoard = new Board();
        _currentSolution = null;
        _currentMoveIndex = 0;

        NextMoveButton.IsEnabled = false;
        PlayAllButton.IsEnabled = false;
        SolutionTimeText.Text = "Solution Time: --";

        MovesListBox.Items.Clear();

        UpdateDisplay();
    }

    private void NextMoveButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentSolution != null && _currentMoveIndex < _currentSolution.Solution.Count)
        {
            Move move = _currentSolution.Solution[_currentMoveIndex];
            _gameBoard.ApplyMove(move);

            // Highlight current move in list
            MovesListBox.SelectedIndex = _currentMoveIndex;

            CurrentMoveText.Text = $"Applied: {move}";
            _currentMoveIndex++;

            if (_currentMoveIndex >= _currentSolution.Solution.Count)
            {
                NextMoveButton.IsEnabled = false;
                PlayAllButton.IsEnabled = false;

                if (_gameBoard.IsSolved())
                {
                    MessageBox.Show("Congratulations! The puzzle has been solved!",
                                   "Puzzle Solved", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            UpdateDisplay();
        }
    }

    private async void PlayAllButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentSolution == null) return;

        PlayAllButton.IsEnabled = false;
        NextMoveButton.IsEnabled = false;

        try
        {
            while (_currentMoveIndex < _currentSolution.Solution.Count)
            {
                Move move = _currentSolution.Solution[_currentMoveIndex];
                _gameBoard.ApplyMove(move);

                // Highlight current move in list
                MovesListBox.SelectedIndex = _currentMoveIndex;
                CurrentMoveText.Text = $"Applied: {move}";
                _currentMoveIndex++;

                UpdateDisplay();

                // Add delay to visualize moves
                await Task.Delay(500);
            }

            if (_gameBoard.IsSolved())
            {
                MessageBox.Show("Congratulations! The puzzle has been solved!",
                               "Puzzle Solved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error playing moves: {ex.Message}",
                           "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}