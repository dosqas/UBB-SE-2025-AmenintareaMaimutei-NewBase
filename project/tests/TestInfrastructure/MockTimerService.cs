// <copyright file="MockTimerService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests.TestInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CourseApp.Services;

    /// <summary>
    /// A mock implementation of the <see cref="ITimerService"/> interface for testing purposes.
    /// </summary>
    public class MockTimerService : ITimerService
    {
        /// <summary>
        /// Occurs when the timer ticks.
        /// </summary>
        public event EventHandler? Tick;

        /// <summary>
        /// Gets or sets the interval between timer ticks.
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Gets a value indicating whether the timer is running.
        /// </summary>
        public bool IsRunning
        {
            get;
            private set;
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            this.IsRunning = true;
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            this.IsRunning = false;
        }

        /// <summary>
        /// Simulates a timer tick for testing purposes.
        /// </summary>
        public void SimulateTick()
        {
            if (this.IsRunning)
            {
                this.Tick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
