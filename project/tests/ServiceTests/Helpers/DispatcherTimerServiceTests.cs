namespace Tests.ServiceTests.Helpers
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
            mockTimer = new Mock<IDispatcherTimer>();
            mockTimer.SetupProperty(t => t.Interval);

            // Create the service using the mock timer
            timerService = new DispatcherTimerService(mockTimer.Object);
            eventRaised = false;
        }

        [Fact]
        public void Interval_Property_SetsAndGetsCorrectly()
        {
            var newInterval = TimeSpan.FromMilliseconds(500);

            timerService.Interval = newInterval;

            Assert.Equal(newInterval, timerService.Interval);
            mockTimer.VerifySet(t => t.Interval = newInterval);
        }

        [Fact]
        public void Start_Method_InitiatesTimer()
        {
            timerService.Start();

            mockTimer.Verify(t => t.Start(), Times.Once);
        }

        [Fact]
        public void Stop_Method_HaltsTimer()
        {
            timerService.Stop();

            mockTimer.Verify(t => t.Stop(), Times.Once);
        }

        [Fact]
        public void OnTimerTick_RaisesTickEvent()
        {
            // Arrange: Subscribe to the Tick event
            timerService.Tick += (sender, e) => eventRaised = true;

            // Act: Fire the Tick event using Moq's Raise method
            mockTimer.Raise(t => t.Tick += null, this, EventArgs.Empty);

            // Assert: Ensure that the event handler was triggered
            Assert.True(eventRaised);
        }

        [Fact]
        public void Dispose_CleansUpEventHandlerAndStopsTimer()
        {
            // Act: Dispose the service  
            timerService.Dispose();

            // Assert: Verify that the OnTimerTick event handler is removed  
            mockTimer.VerifyRemove(t => t.Tick -= It.IsAny<EventHandler<object>>(), Times.Once);

            // Assert: Verify that the timer is stopped  
            mockTimer.Verify(t => t.Stop(), Times.Once);
        }
    }
}
