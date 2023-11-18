using System;
using System.Windows.Input;

namespace WPFSample;

public class Command(Action<object> command) : ICommand
{
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        command(parameter);
    }
}
