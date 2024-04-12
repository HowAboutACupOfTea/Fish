using FishInterpreter.Exe;
using FishInterpreter.Lib;

SortedList<Position, char> code = CodeExecution.ConvertFromStringArray(args);
CodeExecution executer = new(code);

FishRenderer fishRenderer = new();
fishRenderer.RenderCode(code);
executer.StackChangedEvent += fishRenderer.RenderStack;
executer.RegisterChangedEvent += fishRenderer.RenderRegister;
executer.InstructionPointerMovedEvent += fishRenderer.RenderInstructionPointerMovement;

executer.ExecuteCode();
