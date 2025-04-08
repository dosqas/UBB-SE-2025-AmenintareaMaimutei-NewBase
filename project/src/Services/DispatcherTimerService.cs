using System;
using Microsoft.UI.Xaml;

namespace CourseApp.Services
{
    public class DispatcherTimerService : ITimerService
    {
        private readonly DispatcherTimer timer;
        private const int DefaultIntervalMilliseconds = 1000; // 1 second default

        public DispatcherTimerService()
        {
            timer = new ()
            {
                Interval = TimeSpan.FromMilliseconds(DefaultIntervalMilliseconds)
            };
            timer.Tick += OnTimerTick!;
        }

        public event EventHandler? Tick;

        public TimeSpan Interval
        {
            get => timer.Interval;
            set => timer.Interval = value;
        }

        public void Start() => timer.Start();
        public void Stop() => timer.Stop();

        private void OnTimerTick(object sender, object e) => Tick?.Invoke(this, EventArgs.Empty);

        public void Dispose()
        {
            timer.Tick -= OnTimerTick!;
            timer.Stop();
        }
    }
}