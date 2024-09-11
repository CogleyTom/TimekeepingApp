using System;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;

    // The CanExecuteChanged event handler automatically re-evaluates CanExecute when needed
    public event EventHandler CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }

    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        // Evaluate if the command can execute based on the provided predicate function
        return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        // Ensure that the execute delegate is not null before invoking
        _execute?.Invoke(parameter);
    }

    // Method to manually raise the CanExecuteChanged event
    public void RaiseCanExecuteChanged()
    {
        // Safely invoke CanExecuteChanged if subscribed
        CommandManager.InvalidateRequerySuggested();
    }
}
