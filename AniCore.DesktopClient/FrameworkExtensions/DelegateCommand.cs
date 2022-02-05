using System;
using System.Windows.Input;

namespace AniCore.DesktopClient.FrameworkExtensions
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public DelegateCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) =>
            _canExecute is null || _canExecute();

        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) 
                return;

            _execute();
        }

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public event EventHandler? CanExecuteChanged;
    }
}
