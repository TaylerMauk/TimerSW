using System;
using System.Windows;
using System.Windows.Input;

namespace TimerSW.ViewModels
{
    class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private readonly Action m_action;
        private readonly Func<bool> m_canExecuteAction;

        public RelayCommand(Action action, Func<bool> canExecuteAction)
        {
            m_action = action;
            m_canExecuteAction = canExecuteAction;
        }

        public bool CanExecute(object parameter)
        {
            return m_canExecuteAction();
        }

        public void Execute(object parameter)
        {
            m_action();
        }

        public void RaiseCanExecuteChanged()
        {
            // Must be raised on UI thread
            Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        }
    }
}
