using System;
using System.Windows.Input;

namespace MS.BuildTool.WPF
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _executionMethod;

        public DelegateCommand(Action executionMethod)
        {
            _executionMethod = executionMethod;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _executionMethod.Invoke();
        }
    }
}
