using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace TimerSW.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event
        /// </summary>
        /// <param name="callerName">This will be automatically populated with the caller's name</param>
        protected void OnPropertyChanged([CallerMemberName] string callerName = null)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(callerName));

                // Must be raised on UI thread
                Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
            }
        }
    }
}
