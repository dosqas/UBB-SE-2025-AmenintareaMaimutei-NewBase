using CourseApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestInfrastructure
{
    public class MockTimerService : ITimerService
    {
        public event EventHandler Tick;
        public TimeSpan Interval { get; set; }

        public void Start() { IsRunning = true; }
        public void Stop() { IsRunning = false; }

        public bool IsRunning { get; private set; }

        // Helper method to simulate ticks in tests
        public void SimulateTick()
        {
            if (IsRunning)
            {
                Tick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
