using System.Windows.Input;
using TimerSW.Model;

namespace TimerSW.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private bool m_isTicking;
        public bool IsTicking
        {
            get => m_isTicking;
            set
            {
                m_isTicking = value;
                OnPropertyChanged();
            }
        }

        private bool m_isExpired;
        public bool IsExpired
        {
            get => m_isExpired;
            set
            {
                m_isExpired = value;
                OnPropertyChanged();
            }
        }

        private int m_startingSeconds;
        public int StartingSeconds
        {
            get => m_startingSeconds;
            set
            {
                m_startingSeconds = value;
                OnPropertyChanged();
            }
        }

        private int m_startingMinutes;
        public int StartingMinutes
        {
            get => m_startingMinutes;
            set
            {
                m_startingMinutes = value;
                OnPropertyChanged();
            }
        }

        private int m_startingHours;
        public int StartingHours
        {
            get => m_startingHours;
            set
            {
                m_startingHours = value;
                OnPropertyChanged();
            }
        }

        private int m_seconds;
        public int Seconds
        {
            get => m_seconds;
            set
            {
                m_seconds = value;
                OnPropertyChanged();
            }
        }

        private int m_minutes;
        public int Minutes
        {
            get => m_minutes;
            set
            {
                m_minutes = value;
                OnPropertyChanged();
            }
        }

        private int m_hours;
        public int Hours
        {
            get => m_hours;
            set
            {
                m_hours = value;
                OnPropertyChanged();
            }
        }

        private double m_volume;
        public double Volume
        {
            get => m_volume;
            set
            {
                m_volume = value;
                OnPropertyChanged();
            }
        }

        private string m_timeString;
        public string TimeString
        {
            get => m_timeString;
            set
            {
                m_timeString = value;
                OnPropertyChanged();
            }
        }

        private bool m_isBeingConfigured;
        public bool IsBeingConfigured
        {
            get => m_isBeingConfigured;
            set
            {
                m_isBeingConfigured = value;
                OnPropertyChanged();
            }
        }

        private TimerZone m_zone;
        public TimerZone Zone
        {
            get => m_zone;
            set
            {
                m_zone = value;
                OnPropertyChanged();
            }
        }

        private TimerType m_type;
        public TimerType Type
        {
            get => m_type;
            set
            {
                m_type = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartTimerCommand { get; set; }
        public ICommand PauseTimerCommand { get; set; }
        public ICommand ResetTimerCommand { get; set; }
        public ICommand ConfigureTimerCommand { get; set; }
        public ICommand SaveTimerSettingsCommand { get; set; }

        private TimerAgent m_timerAgent;

        public MainViewModel()
        {
            StartTimerCommand = new RelayCommand(StartTimer, CanExecuteStartTimer);
            PauseTimerCommand = new RelayCommand(PauseTimer, CanExecutePauseTimer);
            ResetTimerCommand = new RelayCommand(ResetTimer, CanExecuteResetTimer);
            ConfigureTimerCommand = new RelayCommand(ConfigureTimer, CanExecuteConfigureTimer);
            SaveTimerSettingsCommand = new RelayCommand(SaveTimerSettings, CanExecuteSaveTimerSettings);

            m_timerAgent = new TimerAgent();
            m_timerAgent.Tick += (sender, e) => SyncTimerState();

            m_timerAgent.LoadSettingsFromFile();
            SyncTimerInitState();

            IsBeingConfigured = false;
        }

        private void SyncTimerInitState()
        {
            Type = m_timerAgent.Settings.Type;
            StartingSeconds = m_timerAgent.Settings.StartingSeconds;
            StartingMinutes = m_timerAgent.Settings.StartingMinutes;
            StartingHours = m_timerAgent.Settings.StartingHours;
            Volume = m_timerAgent.Settings.Volume;

            SyncTimerState();
        }

        private void SyncTimerState()
        {
            IsTicking = m_timerAgent.IsTicking;
            IsExpired = m_timerAgent.IsExpired;
            Seconds = m_timerAgent.Seconds;
            Minutes = m_timerAgent.Minutes;
            Hours = m_timerAgent.Hours;
            TimeString = $"{m_hours:00}:{m_minutes:00}:{m_seconds:00}";
            Zone = m_timerAgent.Zone;
        }

        private void StartTimer()
        {
            m_timerAgent.Start();
        }

        private bool CanExecuteStartTimer()
        {
            return !m_isTicking & !m_isBeingConfigured;
        }

        private void PauseTimer()
        {
            m_timerAgent.Pause();
        }

        private bool CanExecutePauseTimer()
        {
            return m_isTicking && !m_isBeingConfigured;
        }

        private void ResetTimer()
        {
            m_timerAgent.Reset();
        }

        private bool CanExecuteResetTimer()
        {
            return !m_isBeingConfigured;
        }

        private void ConfigureTimer()
        {
            IsBeingConfigured = !m_isBeingConfigured;
        }

        private bool CanExecuteConfigureTimer()
        {
            return !m_isTicking;
        }

        private void SaveTimerSettings()
        {
            if (m_type == TimerType.Up)
            {
                StartingSeconds = 0;
                StartingMinutes = 0;
                StartingHours = 0;
            }

            m_timerAgent = new TimerAgent(m_type, m_startingSeconds, m_startingMinutes, m_startingHours);
            m_timerAgent.Tick += (sender, e) => SyncTimerState();
            m_timerAgent.Volume = m_volume;
            SyncTimerInitState();

            m_timerAgent.WriteSettingsToFile();
            IsBeingConfigured = false;
        }

        private bool CanExecuteSaveTimerSettings()
        {
            return true;
        }
    }
}
