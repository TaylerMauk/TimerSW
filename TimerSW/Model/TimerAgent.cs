using System;
using System.IO;
using System.Text.Json;

namespace TimerSW.Model
{
    class TimerAgent
    {
        public readonly string SettingsFilePath;

        public double Volume
        {
            get { return Settings.Volume; }
            set { Settings.Volume = value; }
        }

        public int TotalSeconds { get; private set; }
        public int Seconds { get; private set; }
        public int Minutes { get; private set; }
        public int Hours { get; private set; }

        public bool IsExpired { get; private set; }
        public bool IsTicking { get; private set; }

        public TimerZone Zone { get; private set; }
        public TimerSettings Settings { get; private set; }

        public event EventHandler Tick;

        private System.Timers.Timer m_timer;

        public TimerAgent(TimerType type, int startingSeconds, int startingMinutes, int startingHours)
        {
            Settings = new TimerSettings(type, startingSeconds, startingMinutes, startingHours);
            SettingsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TimerSW",
                "config.json"
            );

            m_timer = new System.Timers.Timer(1000);
            m_timer.Elapsed += (sender, e) => UpdateTimeFields();
            Reset();
        }

        public TimerAgent(TimerType type) :
            this(type, 0, 0, 0)
        { }

        public TimerAgent() :
            this(TimerType.Up, 0, 0, 0)
        { }

        public void LoadSettingsFromFile()
        {
            if (File.Exists(SettingsFilePath))
            {
                Settings = JsonSerializer.Deserialize<TimerSettings>(
                    File.ReadAllText(SettingsFilePath)
                );

                Reset();
            }
        }

        public async void WriteSettingsToFile()
        {
            if (!File.Exists(SettingsFilePath))
            {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(SettingsFilePath)
                );
            }

            using (FileStream f = File.Create(SettingsFilePath))
            {
                await JsonSerializer.SerializeAsync(f, Settings);
            }
        }

        public void Start()
        {
            m_timer.Start();
            IsTicking = true;
            OnTick(null);
        }

        public void Reset()
        {
            m_timer.Stop();

            IsTicking = false;
            IsExpired = false;

            Zone = TimerZone.Safe;
            Seconds = Settings.StartingSeconds;
            Minutes = Settings.StartingMinutes;
            Hours = Settings.StartingHours;
            ComputeTotalSeconds();

            if (Settings.Type == TimerType.Down)
                Settings.WarningThreshold = TotalSeconds / 4;
            else
                Settings.WarningThreshold = -1;

            OnTick(null);
        }

        public void Pause()
        {
            m_timer.Stop();
            IsTicking = false;
            OnTick(null);
        }

        protected virtual void OnTick(EventArgs e)
        {
            Tick?.Invoke(this, e);
        }

        private void UpdateTimeFields()
        {
            switch (Settings.Type)
            {
                case TimerType.Down:
                    UpdateCountDown();
                    break;
                case TimerType.Up:
                    UpdateCountUp();
                    break;
            }

            OnTick(null);
        }

        private void UpdateCountDown()
        {
            --TotalSeconds;
            if (TotalSeconds > 0)
            {
                ComputeTimeFields();

                if (TotalSeconds == Settings.WarningThreshold)
                    Zone = TimerZone.Warning;
            }
            else
            {
                Expire();
            }
        }

        private void UpdateCountUp()
        {
            ++TotalSeconds;
            ComputeTimeFields();
        }

        private void Expire()
        {
            m_timer.Stop();
            IsTicking = false;
            IsExpired = true;
            TotalSeconds = 0;
            Seconds = 0;
            Minutes = 0;
            Hours = 0;
            Zone = TimerZone.Danger;
            OnTick(null);
        }

        private void ComputeTotalSeconds()
        {
            TotalSeconds = Seconds + Minutes * 60 + Hours * 60 * 60;
        }

        private void ComputeTimeFields()
        {
            int seconds = TotalSeconds;

            Seconds = seconds % 60;

            seconds /= 60;
            Minutes = seconds % 60;

            seconds /= 60;
            Hours = seconds % 60;

            //seconds /= 24;
            //Days = seconds % 24;

            //seconds /= 365;
            //Years = seconds % 365;
        }
    }
}
