using System.Windows.Input;

public interface IRelayCommand : ICommand
{
    void RaiseCanExecuteChanged();
}