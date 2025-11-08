namespace PegSolitaireSolver.DataModel;

public struct Move(int fromRow, int fromCol, int toRow, int toCol)
{
    public int FromRow = fromRow, FromCol = fromCol, ToRow = toRow, ToCol = toCol;

    public override readonly string ToString()
    {
        return $"Move peg from ({FromRow},{FromCol}) to ({ToRow},{ToCol})";
    }
}