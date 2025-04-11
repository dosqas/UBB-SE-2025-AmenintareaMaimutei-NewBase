// <copyright file="DispatcherTimerServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests.ServiceTests
{
    using System;
    using CourseApp.Services;
    using Moq;
    using Tests.TestInfrastructure;
    using Xunit;

    /// <summary>
    /// Contains unit tests for the <see cref="DispatcherTimerService"/> class.
    /// </summary>
    public class DispatcherTimerServiceTests
    {
        private readonly Mock<ITestDispatcherTimer> mockTimer;
        private readonly TestableDispatcherTimerService timerService;
        private bool eventRaised;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherTimerServiceTests"/> class.
        /// </summary>
        public DispatcherTimerServiceTests()
        {
            this.mockTimer = new Mock<ITestDispatcherTimer>();
            this.mockTimer.SetupProperty(t => t.Interval);
            this.timerService = new TestableDispatcherTimerService(this.mockTimer.Object);
            this.eventRaised = false;
        }

        /// <summary>
        /// Verifies that the constructor initializes the timer service with the default interval.
        /// </summary>
        [Fact]
        public void Constructor_InitializesWithDefaultInterval()
        {
            // Assert
            Assert.Equal(TimeSpan.FromMilliseconds(1000), this.timerService.Interval);
            this.mockTimer.VerifySet(t => t.Interval = TimeSpan.FromMilliseconds(1000));
        }

        /// <summary>
        /// Verifies that the Interval property sets and gets the correct value.
        /// </summary>
        [Fact]
        public void Interval_Property_SetsAndGetsCorrectly()
        {
            // Arrange
            var newInterval = TimeSpan.FromMilliseconds(500);

            // Act
            this.timerService.Interval = newInterval;

            // Assert
            Assert.Equal(newInterval, this.timerService.Interval);
            this.mockTimer.VerifySet(t => t.Interval = newInterval);
        }

        /// <summary>
        /// Verifies that the Start method initiates the timer.
        /// </summary>
        [Fact]
        public void Start_Method_InitiatesTimer()
        {
            // Act
            this.timerService.Start();

            // Assert
            this.mockTimer.Verify(t => t.Start(), Times.Once);
        }

        /// <summary>
        /// Verifies that the Stop method halts the timer.
        /// </summary>
        [Fact]
        public void Stop_Method_HaltsTimer()
        {
            // Act
            this.timerService.Stop();

            // Assert
            this.mockTimer.Verify(t => t.Stop(), Times.Once);
        }

        /// <summary>
        /// Verifies that the OnTimerTick method raises the Tick event.
        /// </summary>
        [Fact]
        public void OnTimerTick_RaisesTickEvent()
        {
            // Arrange
            this.timerService.Tick += (sender, e) => this.eventRaised = true;

            // Act
            this.timerService.SimulateTimerTick();

            // Assert
            Assert.True(this.eventRaised);
        }

        /// <summary>
        /// Verifies that the Dispose method cleans up event handlers and stops the timer.
        /// </summary>
        [Fact]
        public void Dispose_CleansUpEventHandlersAndStopsTimer()
        {
            // Act
            this.timerService.Dispose();

            // Assert
            this.mockTimer.Verify(t => t.Stop(), Times.Once);
        }

        /// <summary>
        /// Verifies that the Tick event supports multiple subscribers.
        /// </summary>
        [Fact]
        public void TickEvent_SupportsMultipleSubscribers()
        {
            // Arrange
            int eventCount = 0;
            this.timerService.Tick += (sender, e) => eventCount++;
            this.timerService.Tick += (sender, e) => eventCount++;

            // Act
            this.timerService.SimulateTimerTick();

            // Assert
            Assert.Equal(2, eventCount);
        }

        /// <summary>
        /// Verifies that setting the Interval property to zero or a negative value throws an ArgumentOutOfRangeException.
        /// </summary>
        /// <param name="milliseconds">The interval in milliseconds.</param>
        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void Interval_SetToZeroOrNegative_ThrowsArgumentOutOfRangeException(int milliseconds)
        {
            // Arrange
            var interval = TimeSpan.FromMilliseconds(milliseconds);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => this.timerService.Interval = interval);
        }
    }
}