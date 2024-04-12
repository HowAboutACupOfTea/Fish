namespace FishInterpreter.Lib;

internal class FishStack
{
    internal Stack<KeyValuePair<Stack<double>, double?>> _currentlyUnusedStacks;
    internal Stack<double> _currentStack;
    internal double? _currentRegister;

    public FishStack()
    {
        _currentlyUnusedStacks = new();
        _currentStack = new();
    }

    internal double? CurrentRegister
    {
        get
        {
            return _currentRegister;
        }

        set
        {
            _currentRegister = value;
            RegisterChangedEvent?.Invoke(this, new RegisterChangedEventArgs(CurrentRegister));
        }
    }

    public event EventHandler<RegisterChangedEventArgs>? RegisterChangedEvent;
    public event EventHandler<StackChangedEventArgs>? StackChangedEvent;

    public void ChangeRegisterValue()
    {
        if (CurrentRegister == null)
        {
            CurrentRegister = _currentStack.Pop();
        }
        else
        {
            _currentStack.Push(CurrentRegister.GetValueOrDefault());
        }

        InvokeStackChangedEvent();
    }

    public void Push(double value)
    {
        _currentStack.Push(value);
        InvokeStackChangedEvent();
    }

    public double Pop()
    {
        if (_currentStack.Any())
        {
            double popedValue =_currentStack.Pop();
            InvokeStackChangedEvent();
            return popedValue;
        }
        else
        {
            throw new InvalidOperationException("There is no element on the stack.");
        }
    }

    public void CreateNewStack(int count)
    {
        if (_currentStack.Count < count)
            throw new InvalidOperationException($"There are not enough elements on the stack for {nameof(CreateNewStack)}.");

        var tempStackRegisterPair = new KeyValuePair<Stack<double>, double?>(_currentStack, CurrentRegister);
        _currentStack = new();
        CurrentRegister = new();

        for (int i = 0; i < count; i++)
        {
            _currentStack.Push(tempStackRegisterPair.Key.Pop());
        }

        _currentlyUnusedStacks.Push(tempStackRegisterPair);
        InvokeStackChangedEvent();
    }

    public void RemoveStack()
    {
        if (_currentlyUnusedStacks.Count == 0)
        {
            _currentlyUnusedStacks = new();
            return;
        }

        var newCurrentStack = _currentlyUnusedStacks.Pop();

        foreach (var item in _currentStack)
        {
            newCurrentStack.Key.Push(item);
        }

        _currentStack = newCurrentStack.Key;
        CurrentRegister = newCurrentStack.Value;
        InvokeStackChangedEvent();
    }

    public int Count()
    {
        return _currentStack.Count;
    }

    public void ShiftStackRightwards()
    {
        List<double> values = new();

        while (_currentStack.Any())
        {
            values.Add(_currentStack.Pop());
        }

        double firstValue = values.First();
        values.RemoveAt(0);

        while (values.Any())
        {
            _currentStack.Push(values.First());
            values.RemoveAt(0);
        }

        _currentStack.Push(firstValue);
        InvokeStackChangedEvent();
    }

    public void ShiftStackLeftwards()
    {
        List<double> values = new();

        while (_currentStack.Any())
        {
            values.Add(_currentStack.Pop());
        }

        _currentStack.Push(values.Last());
        values.RemoveAt(values.Count - 1);

        while (values.Any())
        {
            _currentStack.Push(values.First());
            values.RemoveAt(0);
        }

        InvokeStackChangedEvent();
    }

    public void ReverseStack()
    {
        Stack<double> tempStack = new();

        while (_currentStack.Any())
        {
            tempStack.Push(_currentStack.Pop());
        }

        while (tempStack.Any())
        {
            _currentStack.Push(tempStack.Pop());
        }

        InvokeStackChangedEvent();
    }

    public void ShiftTopThreeValuesRightwards()
    {
        if (_currentStack.Count < 3)
            throw new InvalidOperationException($"There are not enough elements on the stack for {nameof(ShiftTopThreeValuesRightwards)}.");

        double firstValue = _currentStack.Pop();
        double secondValue = _currentStack.Pop();
        double thirdValue = _currentStack.Pop();
        _currentStack.Push(firstValue);
        _currentStack.Push(thirdValue);
        _currentStack.Push(secondValue);

        InvokeStackChangedEvent();
    }

    public void SwapTopTwoValues()
    {
        if (_currentStack.Count < 2)
            throw new InvalidOperationException($"There are not enough elements on the stack for {nameof(SwapTopTwoValues)}.");

        double firstValue = _currentStack.Pop();
        double secondValue = _currentStack.Pop();
        _currentStack.Push(secondValue);
        _currentStack.Push(firstValue);

        InvokeStackChangedEvent();
    }

    private void InvokeStackChangedEvent()
    {
        StackChangedEvent?.Invoke(this, new StackChangedEventArgs(_currentStack.ToArray()));
    }
}
