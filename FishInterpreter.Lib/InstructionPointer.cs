namespace FishInterpreter.Lib;

internal class InstructionPointer
{
    Position _position = new(0, 0);

    public InstructionPointer()
    {
        Direction = Direction.East;
    }

    public event EventHandler<InstructionPointerMovedEventArgs>? InstructionPointerMovedEvent;

    public Position Position
    {
        get
        {
            return _position;
        }

        internal set
        {
            if(value.X < 0 || value.Y < 0)
            {
                throw new ArgumentException($"The {nameof(InstructionPointer)} is not allowed to be on a negative side {nameof(Position)}.");
            }

            _position = value;
        }
    }

    public Direction Direction
    {
        get;
        internal set;
    }

    public void Move(int x, int y)
    {
        Position oldIPPosition = Position;
        Position = new Position(x, y);
        InstructionPointerMovedEvent?.Invoke(this, new InstructionPointerMovedEventArgs(oldIPPosition, Position));
    }

    public void MoveNext()
    {
        Position oldIPPosition = Position;
        int newX = Position.X + Direction.X;
        int newY = Position.Y + Direction.Y;

        if (newX < 0)
            newX = 0;

        if (Position.Y < 0)
            newY = 0;

        Position = new Position(newX, newY);
        InstructionPointerMovedEvent?.Invoke(this, new InstructionPointerMovedEventArgs(oldIPPosition, Position));
    }

    public void ChangeDirection(Direction newDirection)
    {
        Direction = newDirection;
    }

    public void SetToOppositeDirection()
    {
        Direction =  new Direction(Direction.X * -1, Direction.Y * -1);
    }

    public void SetToVerticalOppositeDirection()
    {
        if (Direction == Direction.North || Direction == Direction.South)
        {
            SetToOppositeDirection();
        }
    }

    public void SetToHorizontalOppositeDirection()
    {
        if (Direction == Direction.East || Direction == Direction.West)
        {
            SetToOppositeDirection();
        }
    }

    public void SetToRandomDirection()
    {
        Direction = GetRandomDirection();
    }

    public void SetDirectionWithSlash()
    {
        if (Direction.X + Direction.Y >= 1)
        {
            Direction = new Direction(Direction.X - 1, Direction.Y - 1);
        }
        else if (Direction.X + Direction.Y <= -1)
        {
            Direction = new Direction(Direction.X + 1, Direction.Y + 1);
        }
        else
        {
            throw new InvalidOperationException($"The current direction {Direction} didn't get changed by {nameof(SetDirectionWithSlash)}.");
        }
    }

    public void SetDirectionWithBackSlash()
    {
        if (Direction.X < Direction.Y)
        {
            Direction = new Direction(Direction.X + 1, Direction.Y - 1);
        }
        else if (Direction.X > Direction.Y)
        {
            Direction = new Direction(Direction.X - 1, Direction.Y + 1);
        }
        else
        {
            throw new InvalidOperationException($"The current direction {Direction} didn't get changed by {nameof(SetDirectionWithBackSlash)}.");
        }
    }

    private static Direction GetRandomDirection()
    {
        Random random = new();

        return random.Next(0, 3) switch
        {
            0 => Direction.North,
            1 => Direction.South,
            2 => Direction.West,
            3 => Direction.East,
            _ => throw new Exception("something smells fishy..."),
        };
    }
}
