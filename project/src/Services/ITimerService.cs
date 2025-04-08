using System;

namespace CourseApp.Services
{
    public interface ITimerService
    {
        void Start();
        void Stop();
        event EventHandler Tick;
        TimeSpan Interval { get; set; }
    }
}
