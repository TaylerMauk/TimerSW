namespace TimerSW.Model
{
    class TimerSettings
    {
        public TimerType Type { get; set; }

        public int StartingSeconds { get; set; }
        public int StartingMinutes { get; set; }
        public int StartingHours { get; set; }
        public int WarningThreshold { get; set; } = -1;

        public TimerSettings(TimerType type, int startingSeconds, int startingMinutes, int startingHours)
        {
            Type = type;
            StartingSeconds = startingSeconds;
            StartingMinutes = startingMinutes;
            StartingHours = startingHours;
        }
    }
}
