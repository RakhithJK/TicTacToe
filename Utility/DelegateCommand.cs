using System;
using System.Windows.Input;

namespace TicTacToe
{
    public class DelegateCommand : ICommand
    {
        readonly Action _execute;
        readonly Func<bool> _onCanExecute;
        
        public DelegateCommand(Action OnExecute, Func<bool> OnCanExecute = null)
        {
            _execute = OnExecute;
            _onCanExecute = OnCanExecute;
        }

        public bool CanExecute(object parameter) => _onCanExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute?.Invoke();
        
        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}