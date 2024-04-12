using FishInterpreter.Lib;

namespace FishInterpreter.Exe;

public class FishRenderer
{
    private readonly int _codeOffsetX = 0;
    private readonly int _codeOffsetY = 3;
    private int _lastRegisterLength = 0;
    private int _lastStackLength = 0;
    private char _lastCodeSnippet;

    public FishRenderer()
    {
        WriteHeadings();
        MovementDelay = 200;
    }

    public int MovementDelay
    {
        get;
        set;
    }

    public void RenderCode(SortedList<Position, char> code)
    {
        foreach (var item in code)
        {
            WriteCharAtPosition(item.Value, item.Key.X, item.Key.Y);
        }

        _lastCodeSnippet = code.First().Value;
    }

    public void RenderStack(object? sender, StackChangedEventArgs args)
    {
        int headingLength = 26;
        ClearPartOfLine(headingLength, 1, _lastStackLength);
        int stackLength = 0;
        Console.SetCursorPosition(headingLength, 1);

        foreach (var item in args.StackContent)
        {
            Console.Write(item);
            stackLength += item.ToString().Length;
        }

        _lastStackLength = stackLength;
    }

    public void RenderRegister(object? sender, RegisterChangedEventArgs args)
    {
        int headingLength = 30;
        ClearPartOfLine(headingLength, 0, _lastRegisterLength);

        if (args.RegisterContent != null)
        {
            Console.SetCursorPosition(headingLength, 0);
            Console.Write(args.RegisterContent);
            _lastRegisterLength = args.RegisterContent.Value.ToString().Length;
        }
        else
        {
            _lastRegisterLength = 0;
        }
    }

    public void RenderInstructionPointerMovement(object? sender, InstructionPointerMovedEventArgs args)
    {
        WriteCharAtPosition(_lastCodeSnippet, args.OldPosition.X, args.OldPosition.Y);
        Console.BackgroundColor = ConsoleColor.Red;
        WriteCharAtPosition(args.CodeSnippet, args.NewPosition.X, args.NewPosition.Y);
        Console.ResetColor();
        _lastCodeSnippet = args.CodeSnippet;

        Thread.Sleep(MovementDelay);
    }

    private void WriteHeadings()
    {
        string registerHeading = "The content of the register: ";
        Console.SetCursorPosition(0, 0);
        Console.Write(registerHeading);

        string stackHeading = "The content of the stack: ";
        Console.SetCursorPosition(0, 1);
        Console.Write(stackHeading);
    }

    private void ClearPartOfLine(int startPointX, int startPointY, int charactersToDeleteCount)
    {
        Console.SetCursorPosition(startPointX, startPointY);

        for (int i = 0; i < charactersToDeleteCount; i++)
        {
            Console.Write(' ');
        }
    }

    private void WriteCharAtPosition(char character, int x, int y)
    {
        Console.SetCursorPosition(x + _codeOffsetX, y + _codeOffsetY);
        Console.Write(character);
    }
}
