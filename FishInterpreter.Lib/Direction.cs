namespace FishInterpreter.Lib;

public record Direction
{
    private int _x;
    private int _y;

    public static readonly Direction North = new(0, -1);
    public static readonly Direction East = new(1, 0);
    public static readonly Direction South = new(0, 1);
    public static readonly Direction West = new(-1, 0);

    public Direction(int x, int y)
    {
        if (x == 0 && y == 0)
        {
            throw new InvalidOperationException("X and y can't both be zero, that is not a direction.");
        }

        X = x;
        Y = y;
    }

    public int X
    {
        get
        {
            return _x;
        }

        set
        {
            if (value < -1 || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The given value must be between -1 and 1 (both included).");
            }

            _x = value;
        }
    }

    public int Y
    {
        get
        {
            return _y;
        }

        set
        {
            if (value < -1 || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The given value must be between -1 and 1 (both included).");
            }

            _y = value;
        }
    }
}
