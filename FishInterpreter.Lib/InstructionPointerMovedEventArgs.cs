namespace FishInterpreter.Lib;

public record InstructionPointerMovedEventArgs(Position OldPosition, Position NewPosition, char CodeSnippet = ' ')
{
}