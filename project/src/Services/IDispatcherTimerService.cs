using System;

namespace CourseApp.Services
{
    public interface IDispatcherTimerService
    {
        void Start();
        void Stop();
        event EventHandler Tick;
        TimeSpan Interval { get; set; }
    }
}
