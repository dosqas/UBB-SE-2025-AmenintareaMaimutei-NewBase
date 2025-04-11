namespace CourseApp.Tests.ServiceTests
{
    using System;
    using CourseApp.Services;
    using CourseApp.Services.Helpers;
    using Microsoft.UI.Xaml;
    using Moq;
    using Xunit;

    public class DispatcherTimerServiceTests
    {
        private readonly Mock<IDispatcherTimer> mockTimer;
        private readonly DispatcherTimerService timerService;  // Use the actual service now
        private bool eventRaised;

        public DispatcherTimerServiceTests()
        {
            // Set up the mock timer
            this.mockTimer = new Mock<IDispatcherTimer>();
            this.mockTimer.SetupProperty(t => t.Interval);

            // Create the service using the mock timer
            this.timerService = new DispatcherTimerService(this.mockTimer.Object);
            this.eventRaised = false;
        }

        [Fact]
        public void Interval_Property_SetsAndGetsCorrectly()
        {
            var newInterval = TimeSpan.FromMilliseconds(500);

            this.timerService.Interval = newInterval;

            Assert.Equal(newInterval, this.timerService.Interval);
            this.mockTimer.VerifySet(t => t.Interval = newInterval);
        }

        [Fact]
        public void Start_Method_InitiatesTimer()
        {
            this.timerService.Start();

            this.mockTimer.Verify(t => t.Start(), Times.Once);
        }

        [Fact]
        public void Stop_Method_HaltsTimer()
        {
            this.timerService.Stop();

            this.mockTimer.Verify(t => t.Stop(), Times.Once);
        }

        [Fact]
        public void OnTimerTick_RaisesTickEvent()
        {
            // Arrange: Subscribe to the Tick event
            this.timerService.Tick += (sender, e) => this.eventRaised = true;

            // Act: Fire the Tick event using Moq's Raise method
            this.mockTimer.Raise(t => t.Tick += null, this, EventArgs.Empty);

            // Assert: Ensure that the event handler was triggered
            Assert.True(this.eventRaised);
        }

        [Fact]
        public void Dispose_CleansUpEventHandlerAndStopsTimer()
        {
            // Act: Dispose the service  
            this.timerService.Dispose();

            // Assert: Verify that the OnTimerTick event handler is removed  
            this.mockTimer.VerifyRemove(t => t.Tick -= It.IsAny<EventHandler<object>>(), Times.Once);

            // Assert: Verify that the timer is stopped  
            this.mockTimer.Verify(t => t.Stop(), Times.Once);
        }
    }
}
