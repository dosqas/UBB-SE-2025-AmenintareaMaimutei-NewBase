namespace CourseApp.Services
{
    using System;
    using CourseApp.Services.Helpers;

    public class DispatcherTimerService : IDispatcherTimerService
    {
        private readonly IDispatcherTimer timer;
        private const int DefaultIntervalMilliseconds = 1000; // 1 second default

        // Constructor accepts an optional IDispatcherTimer parameter
        public DispatcherTimerService(IDispatcherTimer? timer = null)
        {
            // If the provided timer is null, initialize the internal timer
            this.timer = timer ?? new RealDispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(DefaultIntervalMilliseconds)
            };

            // Subscribe to the Tick event
            this.timer.Tick += OnTimerTick!;
        }

        public event EventHandler? Tick;

        public TimeSpan Interval
        {
            get => timer.Interval;
            set => timer.Interval = value;
        }

        public void Start() => timer.Start();
        public void Stop() => timer.Stop();

        private void OnTimerTick(object sender, object e) => Tick!.Invoke(this, EventArgs.Empty);

        public void Dispose()
        {
            timer.Tick -= OnTimerTick!;
            timer.Stop();
        }
    }
}