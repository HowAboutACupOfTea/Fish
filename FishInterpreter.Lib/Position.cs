namespace FishInterpreter.Lib;

public record Position(int X, int Y) : IComparable<Position>
{
    public int CompareTo(Position? other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (this == other)
        {
            return 0;
        }

        if (Y < other.Y)
        {
            return -1;
        }

        if (Y > other.Y)
        {
            return 1;
        }

        if (X < other.X)
        {
            return -1;
        }

        if (X > other.X)
        {
            return 1;
        }

        throw new Exception("How did this happen?");
    }
}
