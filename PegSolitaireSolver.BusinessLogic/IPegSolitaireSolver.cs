using PegSolitaireSolver.DataModel;

namespace PegSolitaireSolver.BusinessLogic;

public interface IPegSolitaireSolver
{
    SolverResult Solve(Board board);
}