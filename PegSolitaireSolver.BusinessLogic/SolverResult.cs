using PegSolitaireSolver.DataModel;

namespace PegSolitaireSolver.BusinessLogic;

public class SolverResult
{
    public bool IsSolved { get; set; }
    public List<Move> Solution { get; set; } = [];
    public TimeSpan SolutionTime { get; set; }
    public int MoveCount => Solution.Count;
}