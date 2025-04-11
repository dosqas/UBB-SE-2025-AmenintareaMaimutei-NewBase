// <copyright file="ITestDispatcherTimer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests.TestInfrastructure
{
    /// <summary>
    /// Interface for a testable dispatcher timer.
    /// </summary>
    public interface ITestDispatcherTimer
    {
        /// <summary>
        /// Gets or sets the interval at which the timer ticks.
        /// </summary>
        TimeSpan Interval { get; set; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        void Stop();
    }
}
