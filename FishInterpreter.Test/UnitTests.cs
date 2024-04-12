using FishInterpreter.Lib;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Security.Principal;

namespace FishInterpreter.Test;

public class Tests
{
    /// <summary>
    /// If this test fails, all other tests will not work.
    /// </summary>
    [Test]
    public void EndExecution()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);

        //Act
        codeExecution.ExecuteCode();

        //Assert
        Assert.Pass();
    }

    [TestCase('ä')]
    public void UseInvalidInstruction(char invalidInstruction)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), invalidInstruction }
        };

        CodeExecution codeExecution = new(codeToExecute);

        //Act, Assert
        Assert.Throws<FishyException>(codeExecution.ExecuteCode);
    }

    [Test]
    public void PopFromEmptyStack()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), 'o' }
        };

        CodeExecution codeExecution = new(codeToExecute);

        //Act, Assert
        Assert.Throws<FishyException>(codeExecution.ExecuteCode);
    }

    [Test]
    public void PutValueToCodebox()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), 'p' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        int valueToPut = 1;
        codeExecution._fishStack._currentStack.Push(valueToPut);
        codeExecution._fishStack._currentStack.Push(2);
        codeExecution._fishStack._currentStack.Push(3);
        KeyValuePair<Position, char> expectedPair = new(new Position(2, 3), Convert.ToChar(valueToPut));

        //Act
        codeExecution.ExecuteCode();
        KeyValuePair<Position, char> actualPair = codeExecution._codebox.ElementAt(2);

        //Assert
        Assert.That(actualPair, Is.EqualTo(expectedPair));
    }

    [Test]
    public void GetValueFromCodeboxToStack()
    {
        //Arrange
        int expectedValue = 5;
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), 'g' },
            { new Position(2, 0), ';' },
            { new Position(1, 2), expectedValue.ToString().First() }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(1);
        codeExecution._fishStack._currentStack.Push(2);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        Assert.That(actualValue, Is.EqualTo(expectedValue));
    }

    [Test]
    public void ChangeRegisterValue_WithEmptyRegister()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '&' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        double expectedValue = 9;
        codeExecution._fishStack._currentStack.Push(expectedValue);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentRegister.GetValueOrDefault();

        //Assert
        Assert.That(actualValue, Is.EqualTo(expectedValue));
    }

    [Test]
    public void ChangeRegisterValue()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '&' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        double expectedValue = 9;
        codeExecution._fishStack._currentRegister = expectedValue;

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        Assert.That(actualValue, Is.EqualTo(expectedValue));
    }

    [Test]
    public void OutputAsNumber()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), 'n' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        char expectedCharacter = 'ä';
        double expectedNumber = 228;
        codeExecution._fishStack._currentStack.Push(expectedCharacter);

        //Act
        codeExecution.ExecuteCode();
        string actualString = codeExecution.StandardOutput;
        double actualNumber = Convert.ToDouble(actualString);

        //Assert
        Assert.That(actualNumber, Is.EqualTo(expectedNumber));
    }

    [Test]
    public void OutputAsCharacter()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), 'o' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        char expectedCharacter = 'ä';
        codeExecution._fishStack._currentStack.Push(expectedCharacter);

        //Act
        codeExecution.ExecuteCode();
        string actualCharacter = codeExecution.StandardOutput;

        //Assert
        Assert.That(actualCharacter, Is.EqualTo(expectedCharacter.ToString()));
    }

    [Test]
    public void PushStdinToStack()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), 'i' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        char expectedCharacter = 'ä';
        codeExecution.StandardInput.Add(expectedCharacter);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();
        int tempValue = Convert.ToInt32(actualValue);
        char actualCharacter = Convert.ToChar(tempValue);

        //Assert
        Assert.That(actualCharacter, Is.EqualTo(expectedCharacter));
    }

    [Test]
    public void PushStdinToStack_WithoutStdin()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), 'i' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        Assert.That(actualValue, Is.EqualTo(-1));
    }

    [Test]
    public void CreateAndFillNewStack()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '[' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(0);

        //Act
        codeExecution.ExecuteCode();
        int unusedStacksCount = codeExecution._fishStack._currentlyUnusedStacks.Count;

        //Assert
        Assert.That(unusedStacksCount, Is.EqualTo(1));
    }

    [Test]
    public void RemoveStack()
    {
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), ']' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);

        //Act
        codeExecution.ExecuteCode();
        int unusedStacksCount = codeExecution._fishStack._currentlyUnusedStacks.Count;

        //Assert
        Assert.That(unusedStacksCount, Is.EqualTo(0));
    }

    [Test]
    public void AddStackLengthToStack()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), 'l' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);

        //Act
        codeExecution.ExecuteCode();
        int actualSize = codeExecution._fishStack._currentStack.Count;
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(actualSize, Is.EqualTo(1));
            Assert.That(actualValue, Is.EqualTo(0));
        });
    }

    [Test]
    public void ReverseStack()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), 'r' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        List<double> insertedValues = new() { 1, 2, 3, 4 };
        List<double> expectedValues = new() { 4, 3, 2, 1 };

        foreach (var value in insertedValues)
        {
            codeExecution._fishStack._currentStack.Push(value);
        }

        //Act
        codeExecution.ExecuteCode();
        List<double> actualValues = codeExecution._fishStack._currentStack.ToList();

        //Assert
        Assert.That(actualValues, Is.EqualTo(expectedValues));
    }

    [Test]
    public void ShiftStackToTheLeft()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '{' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        List<double> insertedValues = new() { 1, 2, 3, 4 };
        List<double> expectedValues = new() { 2, 3, 4, 1 };

        foreach (var value in insertedValues)
        {
            codeExecution._fishStack._currentStack.Push(value);
        }

        //Act
        codeExecution.ExecuteCode();
        List<double> actualValues = new();

        while (codeExecution._fishStack._currentStack.Any())
        {
            actualValues.Add(codeExecution._fishStack._currentStack.Pop());
        }

        //Assert
        Assert.That(actualValues, Is.EqualTo(expectedValues));
    }

    [Test]
    public void ShiftStackToTheRight()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '}' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        List<double> insertedValues = new() { 1, 2, 3, 4 };
        List<double> expectedValues = new() { 4, 1, 2, 3 };

        foreach (var value in insertedValues)
        {
            codeExecution._fishStack._currentStack.Push(value);
        }

        //Act
        codeExecution.ExecuteCode();
        List<double> actualValues = codeExecution._fishStack._currentStack.ToList();

        //Assert
        Assert.That(actualValues, Is.EqualTo(expectedValues));
    }

    [Test]
    public void ShiftTopThreeValuesToTheRight()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '@' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        List<double> insertedValues = new() { 1, 2, 3, 4 };
        List<double> expectedValues = new() { 3, 2, 4, 1 };

        foreach (var value in insertedValues)
        {
            codeExecution._fishStack._currentStack.Push(value);
        }

        //Act
        List<double> someValues = codeExecution._fishStack._currentStack.ToList();
        
        codeExecution.ExecuteCode();
        List<double> actualValues = codeExecution._fishStack._currentStack.ToList();

        //Assert
        Assert.That(actualValues, Is.EqualTo(expectedValues));
    }

    [Test]
    public void SwapTopTwoValues()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '$' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        List<double> expectedValues = new() { 2, 1 };

        codeExecution._fishStack._currentStack.Push(1);
        codeExecution._fishStack._currentStack.Push(2);

        //Act
        codeExecution.ExecuteCode();
        List<double> actualValues = codeExecution._fishStack._currentStack.ToList();

        //Assert
        Assert.That(actualValues, Is.EqualTo(expectedValues));
    }

    [Test]
    public void RemoveTopValue()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '~' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(1);

        //Act
        codeExecution.ExecuteCode();
        int actualSize = codeExecution._fishStack._currentStack.Count;

        //Assert
        Assert.That(actualSize, Is.EqualTo(0));
    }

    [Test]
    public void DuplicateTopValue()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), ':' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        double expectedValue = 1;
        codeExecution._fishStack._currentStack.Push(expectedValue);

        //Act
        codeExecution.ExecuteCode();
        double originalActualValue = codeExecution._fishStack._currentStack.Pop();
        double duplicatedActualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(originalActualValue, Is.EqualTo(expectedValue));
            Assert.That(duplicatedActualValue, Is.EqualTo(expectedValue));
        });
    }

    [TestCase('\'')]
    [TestCase('"')]
    public void DoStringParsing(char parseCharacter)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), parseCharacter },
            { new Position(2, 0), 'a' },
            { new Position(3, 0), '1' },
            { new Position(4, 0), 'ä' },
            { new Position(5, 0), parseCharacter },
            { new Position(6, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);

        //Act
        codeExecution.ExecuteCode();
        int stackContentCount = codeExecution._fishStack._currentStack.Count;

        //Assert
        Assert.That(stackContentCount, Is.EqualTo(3));
    }

    [TestCase(1, 1, ExpectedResult = 0)]
    [TestCase(1, 2, ExpectedResult = 1)]
    [TestCase(2, 1, ExpectedResult = 0)]
    [TestCase(-1, -1, ExpectedResult = 0)]
    [TestCase(-1, -2, ExpectedResult = 0)]
    [TestCase(-2, -1, ExpectedResult = 1)]
    public double DoLessThen(double firstValue, double secondValue)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '(' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(firstValue);
        codeExecution._fishStack._currentStack.Push(secondValue);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        return actualValue;
    }

    [TestCase(1, 1, ExpectedResult = 0)]
    [TestCase(1, 2, ExpectedResult = 0)]
    [TestCase(2, 1, ExpectedResult = 1)]
    [TestCase(-1, -1, ExpectedResult = 0)]
    [TestCase(-1, -2, ExpectedResult = 1)]
    [TestCase(-2, -1, ExpectedResult = 0)]
    public double DoGreaterThen(double firstValue, double secondValue)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), ')' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(firstValue);
        codeExecution._fishStack._currentStack.Push(secondValue);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        return actualValue;
    }

    [TestCase(1, 1, ExpectedResult = 1)]
    [TestCase(1, 2, ExpectedResult = 0)]
    [TestCase(-1, -1, ExpectedResult = 1)]
    [TestCase(-1, -2, ExpectedResult = 0)]
    public double DoEqualOperation(double firstValue, double secondValue)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '=' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(firstValue);
        codeExecution._fishStack._currentStack.Push(secondValue);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        return actualValue;
    }

    [TestCase(9, 4, ExpectedResult = 1)]
    [TestCase(1, 2, ExpectedResult = 1)]
    [TestCase(-9, 9, ExpectedResult = 0)]
    public double DoModulo(double firstValue, double secondValue)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '%' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(firstValue);
        codeExecution._fishStack._currentStack.Push(secondValue);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        return actualValue;
    }

    [TestCase(9, 4, ExpectedResult = 2.25)]
    public double DoDivision(double firstValue, double secondValue)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), ',' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(firstValue);
        codeExecution._fishStack._currentStack.Push(secondValue);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        return actualValue;
    }

    [TestCase(1, 0)]
    public void DoDivision_ByZero(double firstValue, double secondValue)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), ',' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(firstValue);
        codeExecution._fishStack._currentStack.Push(secondValue);

        //Act, Assert
        Assert.Throws<FishyException>(codeExecution.ExecuteCode);
    }

    [TestCase(1, 2, ExpectedResult = 2)]
    [TestCase(-9, 9, ExpectedResult = -81)]
    public double DoMultiplication(double firstValue, double secondValue)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '*' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(firstValue);
        codeExecution._fishStack._currentStack.Push(secondValue);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        return actualValue;
    }

    [TestCase(1, 2, ExpectedResult = -1)]
    [TestCase(-9, 9, ExpectedResult = -18)]
    public double DoSubtraction(double firstValue, double secondValue)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '-' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(firstValue);
        codeExecution._fishStack._currentStack.Push(secondValue);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        return actualValue;
    }

    [TestCase(1, 2, ExpectedResult = 3)]
    [TestCase(-9, 9, ExpectedResult = 0)]
    [TestCase(9999999999999999999999999999999999.0, -1, ExpectedResult = 9999999999999999999999999999999998.0)]
    public double DoAddition(double firstValue, double secondValue)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '+' },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(firstValue);
        codeExecution._fishStack._currentStack.Push(secondValue);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        return actualValue;
    }

    [TestCase(1)]
    [TestCase(0)]
    public void PushValueOntoStack(double expectedValue)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), expectedValue.ToString().First() },
            { new Position(2, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);

        //Act
        codeExecution.ExecuteCode();
        double actualValue = codeExecution._fishStack._currentStack.Pop();

        //Assert
        Assert.That(actualValue, Is.EqualTo(expectedValue));
    }

    [Test]
    public void Jump()
    {
        //Arrange
        Position jumpToPosition = new(9, 9);
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '.' },
            { jumpToPosition, ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(jumpToPosition.X);
        codeExecution._fishStack._currentStack.Push(jumpToPosition.Y);

        //Act
        codeExecution.ExecuteCode();
        Position actualPosition = codeExecution._instructionPointer.Position;

        //Assert
        Assert.That(actualPosition, Is.EqualTo(new Position(0, 9)));
    }

    [Test]
    public void ConditionalTrampoline()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '?' },
            { new Position(2, 0), '9' },
            { new Position(3, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(1);

        //Act
        codeExecution.ExecuteCode();
        int stackContentCount = codeExecution._fishStack._currentStack.Count;

        //Assert
        Assert.That(stackContentCount, Is.EqualTo(1));
    }

    [Test]
    public void ConditionalTrampoline_WithZero()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '?' },
            { new Position(2, 0), '9' },
            { new Position(3, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._fishStack._currentStack.Push(0);

        //Act
        codeExecution.ExecuteCode();
        int stackContentCount = codeExecution._fishStack._currentStack.Count;

        //Assert
        Assert.That(stackContentCount, Is.EqualTo(0));
    }

    [Test]
    public void Trampoline()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 0), '!' },
            { new Position(2, 0), '9' },
            { new Position(3, 0), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        
        //Act
        codeExecution.ExecuteCode();
        int stackContentCount = codeExecution._fishStack._currentStack.Count;

        //Assert
        Assert.That(stackContentCount, Is.EqualTo(0));
    }

    private readonly static object[] _setDirectionToOpposite_TestCases =
    {
        new object[] { Direction.North, '|', Direction.North },
        new object[] { Direction.East,  '|', Direction.West },
        new object[] { Direction.South, '|', Direction.South },
        new object[] { Direction.West,  '|', Direction.East },
        new object[] { Direction.North, '_', Direction.South },
        new object[] { Direction.East,  '_', Direction.East },
        new object[] { Direction.South, '_', Direction.North },
        new object[] { Direction.West,  '_', Direction.West }
    };

    [TestCaseSource(nameof(_setDirectionToOpposite_TestCases))]
    public void SetDirectionToOpposite(Direction startingDirection, char directionCharacter, Direction expectedDirection)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 1), directionCharacter },
            { new Position(1, 0), ';' },
            { new Position(0, 1), ';' },
            { new Position(2, 1), ';' },
            { new Position(1, 2), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._instructionPointer.Position = new Position(1, 1);
        codeExecution._instructionPointer.Direction = startingDirection;

        //Act
        codeExecution.ExecuteCode();
        Direction actualDirection = codeExecution._instructionPointer.Direction;

        //Assert
        Assert.That(actualDirection, Is.EqualTo(expectedDirection));
    }

    [Test]
    [Repeat(16)]
    public void SetDirectionToRandom()
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 1), 'x' },
            { new Position(1, 0), ';' },
            { new Position(0, 1), ';' },
            { new Position(2, 1), ';' },
            { new Position(1, 2), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._instructionPointer.Position = new Position(1, 1);

        //Act
        codeExecution.ExecuteCode();
        Direction actualDirection = codeExecution._instructionPointer.Direction;

        //Assert
        Assert.That(actualDirection.X + actualDirection.Y, Is.AtMost(2));
        Assert.That(actualDirection.X + actualDirection.Y, Is.AtLeast(-2));
        Assert.That(actualDirection.X + actualDirection.Y, Is.Not.Zero);
    }

    private readonly static object[] _changeDirectionWithSlashes_TestCases =
    {
        new object[] { Direction.North, '/', Direction.East },
        new object[] { Direction.East,  '/', Direction.North },
        new object[] { Direction.South, '/', Direction.West },
        new object[] { Direction.West,  '/', Direction.South },
        new object[] { Direction.North, '\\', Direction.West },
        new object[] { Direction.East, '\\', Direction.South },
        new object[] { Direction.South, '\\', Direction.East },
        new object[] { Direction.West, '\\', Direction.North }
    };

    [TestCaseSource(nameof(_changeDirectionWithSlashes_TestCases))]
    public void ChangeDirectionWithSlashes(Direction startingDirection, char slashCharacter, Direction expectedDirection)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 1), slashCharacter },
            { new Position(1, 0), ';' },
            { new Position(0, 1), ';' },
            { new Position(2, 1), ';' },
            { new Position(1, 2), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._instructionPointer.Position = new Position(1, 1);
        codeExecution._instructionPointer.Direction = startingDirection;

        //Act
        codeExecution.ExecuteCode();
        Direction actualDirection = codeExecution._instructionPointer.Direction;

        //Assert
        Assert.That(actualDirection, Is.EqualTo(expectedDirection));
    }

    private readonly static object[] _changeDirection_TestCases =
    {
        new object[] { '^', Direction.North },
        new object[] { '>', Direction.East },
        new object[] { 'v', Direction.South },
        new object[] { '<', Direction.West }
    };

    [TestCaseSource(nameof(_changeDirection_TestCases))]
    public void ChangeDirection(char directionCharacter, Direction expectedDirection)
    {
        //Arrange
        var codeToExecute = new SortedList<Position, char>
        {
            { new Position(1, 1), directionCharacter },
            { new Position(1, 0), ';' },
            { new Position(0, 1), ';' },
            { new Position(2, 1), ';' },
            { new Position(1, 2), ';' }
        };

        CodeExecution codeExecution = new(codeToExecute);
        codeExecution._instructionPointer.Position = new Position(1, 1);

        //Act
        codeExecution.ExecuteCode();
        Direction actualDirection = codeExecution._instructionPointer.Direction;

        //Assert
        Assert.That(actualDirection, Is.EqualTo(expectedDirection));
    }
}
