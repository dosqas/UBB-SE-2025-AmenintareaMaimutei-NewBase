using System;
using System.Windows.Input;

namespace CourseApp.ViewModels
{
    public class RelayCommand : ICommand, IRelayCommand
    {
        private readonly Action<object?> executeAction;
        private readonly Predicate<object?>? canExecutePredicate;
        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<object?> execute) : this(execute, null) { }

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute)
        {
            executeAction = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecutePredicate = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecutePredicate == null || canExecutePredicate(parameter);
        }

        public void Execute(object? parameter)
        {
            executeAction(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
