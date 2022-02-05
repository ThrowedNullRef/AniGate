using System;
using System.Windows.Input;

namespace AniCore.DesktopClient.FrameworkExtensions;

public class ParameterizedCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public ParameterizedCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute is null || _canExecute((T?)parameter);
    }

    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        _execute((T?)parameter);
    }

    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;
}