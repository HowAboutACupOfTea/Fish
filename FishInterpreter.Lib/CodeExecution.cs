using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FishInterpreter.Test")]

namespace FishInterpreter.Lib;

public class CodeExecution
{
    internal readonly SortedList<Position, char> _codebox = new();
    internal readonly InstructionPointer _instructionPointer = new();
    internal readonly FishStack _fishStack = new();
    internal readonly Dictionary<char, Action> _instructionAssignments = new();
    internal bool _isExecuting;

    public List<char> StandardInput
    {
        get;
        internal set;
    }

    public string StandardOutput
    {
        get;
        internal set;
    }

    public bool IsExecuting
    {
        get
        {
            return _isExecuting;
        }

        set
        {
            _isExecuting = value;

            if (!IsExecuting)
                ExecutionStoppedEvent?.Invoke(this, new EventArgs());
        }
    }

    public CodeExecution(SortedList<Position, char> codebox)
    {
        _codebox = codebox ?? throw new FishyException(new ArgumentNullException(nameof(codebox)));
        AddMovementFunctionality();
        AddExecutionFunctionality();
        AddLiteralFunctionality();
        AddOperatorFunctionality();
        AddStackManipulationFunctionality();
        AddInputAndOutputFunctionality();
        AddReflectionAndMiscellaneousFunctionality();
        _instructionAssignments.Add(' ', MoveInstructionPointerNext);

        StandardInput = new();
        StandardOutput = string.Empty;
        _fishStack.RegisterChangedEvent += InvokeRegisterChangedEvent;
        _fishStack.StackChangedEvent += InvokeStackChangedEvent;
        _instructionPointer.InstructionPointerMovedEvent += InvokeInstructionPointerMovedEvent;
    }

    public event EventHandler<RegisterChangedEventArgs>? RegisterChangedEvent;
    public event EventHandler<StackChangedEventArgs>? StackChangedEvent;
    public event EventHandler<InstructionPointerMovedEventArgs>? InstructionPointerMovedEvent;
    public event EventHandler<EventArgs>? ExecutionStoppedEvent;

    public void ExecuteCode()
    {
        IsExecuting = true;
        
        try
        {
            while (IsExecuting)
            {
                bool containsKey = _codebox.TryGetValue(_instructionPointer.Position, out char key);

                if (containsKey)
                {
                    Action action = _instructionAssignments.GetValueOrDefault(key) ?? throw new InvalidOperationException($"No operation with the key {key} was found.");
                    action.Invoke();
                }

                MoveInstructionPointerNext();
            }
        }
        catch (Exception e)
        {
            throw new FishyException(e);
        }
    }

    public static SortedList<Position, char> ConvertFromStringArray(string[] fishCode)
    {
        SortedList<Position, char> code = new();

        for (int i = 0; i < fishCode.Length; i++)
        {
            string text = fishCode[i];

            for (int j = 0; j < text.Length; j++)
            {
                if (text[j] != ' ')
                {
                    code.Add(new Position(j, i), text[j]);
                }
            }
        }

        return code;
    }

    internal void InvokeRegisterChangedEvent(object? sender, RegisterChangedEventArgs args)
    {
        RegisterChangedEvent?.Invoke(this, args);
    }

    internal void InvokeStackChangedEvent(object? sender, StackChangedEventArgs args)
    {
        StackChangedEvent?.Invoke(this, args);
    }

    internal void InvokeInstructionPointerMovedEvent(object? sender, InstructionPointerMovedEventArgs args)
    {
        bool containsKey = _codebox.TryGetValue(_instructionPointer.Position, out char key);

        if (!containsKey)
            key = ' ';

        InstructionPointerMovedEvent?.Invoke(this, new InstructionPointerMovedEventArgs(args.OldPosition, args.NewPosition, key));
    }
    
    internal void AddReflectionAndMiscellaneousFunctionality()
    {
        _instructionAssignments.Add('&', ChangeRegisterValue);
        _instructionAssignments.Add('g', GetValueFromCodeboxToStack);
        _instructionAssignments.Add('p', PutValueToCodebox);
        _instructionAssignments.Add(';', EndExecution);
    }

    internal void AddInputAndOutputFunctionality()
    {
        _instructionAssignments.Add('i', PushStdinToStack);
        _instructionAssignments.Add('o', OutputAsCharacter);
        _instructionAssignments.Add('n', OutputAsNumber);
    }

    internal void AddStackManipulationFunctionality()
    {
        _instructionAssignments.Add(':', DuplicateTopValue);
        _instructionAssignments.Add('~', RemoveTopValue);
        _instructionAssignments.Add('$', SwapTopTwoValues);
        _instructionAssignments.Add('@', ShiftTopThreeValuesToTheRight);
        _instructionAssignments.Add('}', ShiftStackToTheRight);
        _instructionAssignments.Add('{', ShiftStackToTheLeft);
        _instructionAssignments.Add('r', ReverseStack);
        _instructionAssignments.Add('l', AddStackLengthToStack);
        _instructionAssignments.Add('[', CreateAndFillNewStack);
        _instructionAssignments.Add(']', RemoveStack);
    }

    internal void AddOperatorFunctionality()
    {
        _instructionAssignments.Add('+', DoAddition);
        _instructionAssignments.Add('-', DoSubtraction);
        _instructionAssignments.Add('*', DoMultiplication);
        _instructionAssignments.Add(',', DoDivision);
        _instructionAssignments.Add('%', DoModulo);
        _instructionAssignments.Add('=', DoEqualOperation);
        _instructionAssignments.Add(')', DoGreaterThen);
        _instructionAssignments.Add('(', DoLessThen);
        _instructionAssignments.Add('\'', () =>
        {
            DoStringParsing('\'');
        });

        _instructionAssignments.Add('"', () =>
        {
            DoStringParsing('"');
        });
    }

    internal void AddLiteralFunctionality()
    {
        _instructionAssignments.Add('0', () =>
        {
            PushValueOntoStack(0);
        });

        _instructionAssignments.Add('1', () =>
        {
            PushValueOntoStack(1);
        });

        _instructionAssignments.Add('2', () =>
        {
            PushValueOntoStack(2);
        });

        _instructionAssignments.Add('3', () =>
        {
            PushValueOntoStack(3);
        });

        _instructionAssignments.Add('4', () =>
        {
            PushValueOntoStack(4);
        });

        _instructionAssignments.Add('5', () =>
        {
            PushValueOntoStack(5);
        });

        _instructionAssignments.Add('6', () =>
        {
            PushValueOntoStack(6);
        });

        _instructionAssignments.Add('7', () =>
        {
            PushValueOntoStack(7);
        });

        _instructionAssignments.Add('8', () =>
        {
            PushValueOntoStack(8);
        });

        _instructionAssignments.Add('9', () =>
        {
            PushValueOntoStack(9);
        });

        _instructionAssignments.Add('a', () =>
        {
            PushValueOntoStack(10);
        });

        _instructionAssignments.Add('b', () =>
        {
            PushValueOntoStack(11);
        });

        _instructionAssignments.Add('c', () =>
        {
            PushValueOntoStack(12);
        });

        _instructionAssignments.Add('d', () =>
        {
            PushValueOntoStack(13);
        });

        _instructionAssignments.Add('e', () =>
        {
            PushValueOntoStack(14);
        });

        _instructionAssignments.Add('f', () =>
        {
            PushValueOntoStack(15);
        });
    }

    internal void AddExecutionFunctionality()
    {
        _instructionAssignments.Add('!', Trampoline);
        _instructionAssignments.Add('?', ConditionalTrampoline);
        _instructionAssignments.Add('.', Jump);
    }

    internal void AddMovementFunctionality()
    {
        _instructionAssignments.Add('>', () =>
        {
            ChangeDirection(Direction.East);
        });

        _instructionAssignments.Add('<', () =>
        {
            ChangeDirection(Direction.West);
        });

        _instructionAssignments.Add('^', () =>
        {
            ChangeDirection(Direction.North);
        });

        _instructionAssignments.Add('v', () =>
        {
            ChangeDirection(Direction.South);
        });

        _instructionAssignments.Add('/', ChangeDirectionWithSlash);
        _instructionAssignments.Add('\\', ChangeDirectionWithBackSlash);
        _instructionAssignments.Add('|', SetDirectionToHorizontalOpposite);
        _instructionAssignments.Add('_', SetDirectionToVerticalOpposite);
        _instructionAssignments.Add('#', SetDirectionToOpposite);
        _instructionAssignments.Add('x', SetDirectionToRandom);
    }

    internal void EndExecution()
    {
        IsExecuting = false;
    }

    internal void PutValueToCodebox()
    {
        if (_fishStack.Count() < 3)
            throw new InvalidOperationException($"There are not enough elements on the stack for {nameof(PutValueToCodebox)}.");

        int y = Convert.ToInt32(PopValue());
        int x = Convert.ToInt32(PopValue());
        int valueFromStack = Convert.ToInt32(PopValue());
        char v = Convert.ToChar(valueFromStack);
        _codebox.Add(new Position(x, y), v);
    }

    internal void GetValueFromCodeboxToStack()
    {
        if (_fishStack.Count() < 2)
            throw new InvalidOperationException($"There are not enough elements on the stack for {nameof(GetValueFromCodeboxToStack)}.");

        int y = Convert.ToInt32(PopValue());
        int x = Convert.ToInt32(PopValue());
        bool gotChar = _codebox.TryGetValue(new Position(x, y), out char charToPush);

        if (gotChar)
        {
            double valueToPush = char.GetNumericValue(charToPush);
            PushValueOntoStack(valueToPush);
        }
        else
        {
            PushValueOntoStack(0);
        }
    }

    internal double PopValue()
    {
        return _fishStack.Pop();
    }

    internal void ChangeRegisterValue()
    {
        _fishStack.ChangeRegisterValue();
    }

    internal void OutputAsNumber()
    {
        string numberAsString = PopValue().ToString();
        StandardOutput += numberAsString;
    }

    internal void OutputAsCharacter()
    {
        int valueFromStack = Convert.ToInt32(PopValue());
        char character = Convert.ToChar(valueFromStack);
        StandardOutput += character.ToString();
    }

    internal void PushStdinToStack()
    {
        if (!StandardInput.Any())
        {
            PushValueOntoStack(-1);
        }
        else
        {
            PushValueOntoStack(StandardInput.First());
            StandardInput.RemoveAt(0);
        }
    }

    internal void RemoveStack()
    {
        _fishStack.RemoveStack();
    }

    internal void CreateAndFillNewStack()
    {
        int valuesToMoveCount = Convert.ToInt32(_fishStack.Pop());
        _fishStack.CreateNewStack(valuesToMoveCount);
    }

    internal void AddStackLengthToStack()
    {
        _fishStack.Push(_fishStack.Count());
    }

    internal void ReverseStack()
    {
        _fishStack.ReverseStack();
    }

    internal void ShiftStackToTheLeft()
    {
        _fishStack.ShiftStackLeftwards();
    }

    internal void ShiftStackToTheRight()
    {
        _fishStack.ShiftStackRightwards();
    }

    internal void ShiftTopThreeValuesToTheRight()
    {
        _fishStack.ShiftTopThreeValuesRightwards();
    }

    internal void SwapTopTwoValues()
    {
        _fishStack.SwapTopTwoValues();
    }

    internal void RemoveTopValue()
    {
        PopValue();
    }

    internal void DuplicateTopValue()
    {
        double value = PopValue();
        PushValueOntoStack(value);
        PushValueOntoStack(value);
    }

    internal void DoStringParsing(char parsCharacter)
    {
        MoveInstructionPointerNext();
        char currentChar = _codebox.GetValueOrDefault(_instructionPointer.Position, ' ');

        while (currentChar != parsCharacter)
        {
            PushValueOntoStack(currentChar);
            MoveInstructionPointerNext();
            currentChar = _codebox.GetValueOrDefault(_instructionPointer.Position, ' ');
        }
    }

    internal void DoLessThen()
    {
        double x = PopValue();
        double y = PopValue();
        double result = 0;

        if (y < x)
            result = 1;

        PushValueOntoStack(result);
    }

    internal void DoGreaterThen()
    {
        double x = PopValue();
        double y = PopValue();
        double result = 0;

        if (y > x)
            result = 1;

        PushValueOntoStack(result);
    }

    internal void DoEqualOperation()
    {
        double x = PopValue();
        double y = PopValue();
        double result = 0;

        if (y == x)
            result = 1;

        PushValueOntoStack(result);
    }

    internal void DoModulo()
    {
        double x = PopValue();
        double y = PopValue();
        double result = y % x;
        PushValueOntoStack(result);
    }

    internal void DoDivision()
    {
        double x = PopValue();
        double y = PopValue();

        if (x == 0)
        {
            throw new InvalidOperationException("Can't divide by zero.");
        }

        double result = y / x;
        PushValueOntoStack(result);
    }

    internal void DoMultiplication()
    {
        double x = PopValue();
        double y = PopValue();
        double result = y * x;
        PushValueOntoStack(result);
    }

    internal void DoSubtraction()
    {
        double x = PopValue();
        double y = PopValue();
        double result = y - x;
        PushValueOntoStack(result);
    }

    internal void DoAddition()
    {
        double x = PopValue();
        double y = PopValue();
        double result = y + x;
        PushValueOntoStack(result);
    }

    internal void PushValueOntoStack(double value)
    {
        _fishStack.Push(value);
    }

    internal void Jump()
    {
        double y = PopValue();
        double x = PopValue();
        _instructionPointer.Move(Convert.ToInt32(x), Convert.ToInt32(y));
    }

    internal void ConditionalTrampoline()
    {
        double value = PopValue();

        if (value == 0)
            Trampoline();
    }

    internal void Trampoline()
    {
        MoveInstructionPointerNext();
    }

    internal void SetDirectionToHorizontalOpposite()
    {
        _instructionPointer.SetToHorizontalOppositeDirection();
    }

    internal void SetDirectionToVerticalOpposite()
    {
        _instructionPointer.SetToVerticalOppositeDirection();
    }

    internal void SetDirectionToOpposite()
    {
        _instructionPointer.SetToOppositeDirection();
    }

    internal void SetDirectionToRandom()
    {
        _instructionPointer.SetToRandomDirection();
    }

    internal void ChangeDirectionWithSlash()
    {
        _instructionPointer.SetDirectionWithSlash();
    }

    internal void ChangeDirectionWithBackSlash()
    {
        _instructionPointer.SetDirectionWithBackSlash();
    }

    internal void ChangeDirection(Direction newDirection)
    {
        _instructionPointer.ChangeDirection(newDirection);
    }

    internal bool ShouldPointerWrapAround()
    {
        IList<Position> keys = _codebox.Keys;
        Direction currentDirection = _instructionPointer.Direction;
        int currentX = _instructionPointer.Position.X;
        int currentY = _instructionPointer.Position.Y;

        if (currentDirection == Direction.North)
        {
            return !keys.Any(otherPosition => otherPosition.X == currentX && otherPosition.Y < currentY);
        }
        else if (currentDirection == Direction.South)
        {
            return !keys.Any(otherPosition => otherPosition.X == currentX && otherPosition.Y > currentY);
        }
        else if (currentDirection == Direction.East)
        {
            return !keys.Any(otherPosition => otherPosition.X > currentX && otherPosition.Y == currentY);
        }
        else if (currentDirection == Direction.West)
        {
            return !keys.Any(otherPosition => otherPosition.X < currentX && otherPosition.Y == currentY);
        }

        return true;
    }

    internal void MoveInstructionPointerNext()
    {
        if (!ShouldPointerWrapAround())
        {
            _instructionPointer.MoveNext();
            return;
        }

        int newX = _instructionPointer.Position.X;
        int newY = _instructionPointer.Position.Y;

        if (_instructionPointer.Direction == Direction.North)
        {
            newY = GetHighestY();
        }
        else if (_instructionPointer.Direction == Direction.East)
        {
            newX = 0;
        }
        else if (_instructionPointer.Direction == Direction.South)
        {
            newY = 0;
        }
        else if (_instructionPointer.Direction == Direction.West)
        {
            newX = GetHighestX();
        }

        _instructionPointer.Move(newX, newY);
    }

    internal int GetHighestY()
    {
        return _codebox.Keys.Last().Y;
    }

    internal int GetHighestX()
    {
        int highestX = 0;

        foreach (var position in _codebox.Keys)
        {
            if (position.X > highestX)
            {
                highestX = position.X;
            }
        }

        return highestX;
    }
}
