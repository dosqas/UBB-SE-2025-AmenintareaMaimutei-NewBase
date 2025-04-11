// <copyright file="TestableDispatcherTimerService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests.TestInfrastructure
{
    using System;
    using CourseApp.Services;
    using Tests.ServiceTests;

    /// <summary>
    /// Testable version of a timer service that allows simulating ticks,
    /// without depending on the real DispatcherTimer.
    /// </summary>
    public partial class TestableDispatcherTimerService : ITimerService, IDisposable
    {
        private const int DefaultIntervalMilliseconds = 1000;
        private readonly ITestDispatcherTimer timer;
        private EventHandler? tickHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestableDispatcherTimerService"/> class.
        /// </summary>
        /// <param name="timer">An implementation of <see cref="ITestDispatcherTimer"/>.</param>
        public TestableDispatcherTimerService(ITestDispatcherTimer timer)
        {
            this.timer = timer;
            this.timer.Interval = TimeSpan.FromMilliseconds(DefaultIntervalMilliseconds);
        }

        /// <summary>
        /// Event raised when the timer ticks.
        /// </summary>
        public event EventHandler? Tick
        {
            add { this.tickHandler += value; }
            remove { this.tickHandler -= value; }
        }

        /// <summary>
        /// Gets or sets the interval between ticks.
        /// Throws if the interval is not positive.
        /// </summary>
        public TimeSpan Interval
        {
            get => this.timer.Interval;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Interval must be positive");
                }

                this.timer.Interval = value;
            }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start() => this.timer.Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop() => this.timer.Stop();

        /// <summary>
        /// Simulates a tick event, triggering all Tick event handlers.
        /// </summary>
        public void SimulateTimerTick() => this.tickHandler?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Disposes the service and stops the timer.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
            GC.SuppressFinalize(this);
        }
    }
}
