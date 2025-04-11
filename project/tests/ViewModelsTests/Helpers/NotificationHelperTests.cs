// <copyright file="NotificationHelperTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests.ViewModelsTests.Helpers
{
    using System;
    using CourseApp.Services;
    using CourseApp.ViewModels;
    using CourseApp.ViewModels.Helpers;
    using Moq;
    using Xunit;

    /// <summary>
    /// Contains unit tests for the <see cref="NotificationHelper"/> class.
    /// </summary>
    public class NotificationHelperTests
    {
        private readonly Mock<CourseViewModel> mockParentViewModel;
        private readonly Mock<IDispatcherTimerService> mockTimerService;
        private readonly NotificationHelper notificationHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHelperTests"/> class.
        /// Sets up the test environment with mock objects.
        /// </summary>
        public NotificationHelperTests()
        {
            mockParentViewModel = new Mock<CourseViewModel>();
            mockTimerService = new Mock<IDispatcherTimerService>();
            notificationHelper = new NotificationHelper(mockParentViewModel.Object, mockTimerService.Object);
        }

        /// <summary>
        /// Tests that the constructor throws ArgumentNullException when parentViewModel is null.
        /// </summary>
        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenParentViewModelIsNull()
        {
            // Arrange
            CourseViewModel? nullParentViewModel = null;

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(
                () => new NotificationHelper(nullParentViewModel!, mockTimerService.Object));
        }

        /// <summary>
        /// Tests that the constructor throws ArgumentNullException when timerService is null.
        /// </summary>
        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenTimerServiceIsNull()
        {
            // Arrange
            IDispatcherTimerService? nullTimerService = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new NotificationHelper(mockParentViewModel.Object, nullTimerService!));
        }

        /// <summary>
        /// Tests that the constructor properly subscribes to the timer's Tick event.
        /// </summary>
        [Fact]
        public void Constructor_SubscribesToTimerTickEvent()
        {
            // Arrange
            var timerMock = new Mock<IDispatcherTimerService>();

            // Act
            var helper = new NotificationHelper(mockParentViewModel.Object, timerMock.Object);

            // Assert
            timerMock.VerifyAdd(t => t.Tick += It.IsAny<EventHandler>(), Times.Once);
        }

        /// <summary>
        /// Tests that ShowTemporaryNotification sets the notification message and visibility.
        /// </summary>
        [Fact]
        public void ShowTemporaryNotification_SetsNotificationMessageAndVisibility()
        {
            // Arrange
            const string testMessage = "Test notification";

            // Act
            notificationHelper.ShowTemporaryNotification(testMessage);

            // Assert
            mockParentViewModel.VerifySet(vm => vm.NotificationMessage = testMessage, Times.Once);
            mockParentViewModel.VerifySet(vm => vm.ShowNotification = true, Times.Once);
        }

        /// <summary>
        /// Tests that ShowTemporaryNotification sets the timer interval and starts the timer.
        /// </summary>
        [Fact]
        public void ShowTemporaryNotification_SetsTimerIntervalAndStartsTimer()
        {
            // Arrange
            const string testMessage = "Test notification";
            var expectedInterval = TimeSpan.FromSeconds(CourseViewModel.NotificationDisplayDurationInSeconds);

            // Act
            notificationHelper.ShowTemporaryNotification(testMessage);

            // Assert
            mockTimerService.VerifySet(t => t.Interval = expectedInterval, Times.Once);
            mockTimerService.Verify(t => t.Start(), Times.Once);
        }

        /// <summary>
        /// Tests that the timer tick event handler hides the notification and stops the timer.
        /// </summary>
        [Fact]
        public void OnNotificationTimerTick_HidesNotificationAndStopsTimer()
        {
            // Arrange
            var eventArgs = EventArgs.Empty;

            // Act
            // Simulate timer tick by invoking the event handler directly
            mockTimerService.Raise(t => t.Tick += null, this, eventArgs);

            // Assert
            mockParentViewModel.VerifySet(vm => vm.ShowNotification = false, Times.Once);
            mockTimerService.Verify(t => t.Stop(), Times.Once);
        }

        /// <summary>
        /// Tests that the timer tick event handler doesn't throw when parent view model is null.
        /// </summary>
        [Fact]
        public void OnNotificationTimerTick_DoesNotThrow_WhenParentViewModelIsNull()
        {
            // Arrange
            var eventArgs = EventArgs.Empty;
            var timerMock = new Mock<IDispatcherTimerService>();
            var helper = new NotificationHelper(mockParentViewModel.Object, timerMock.Object);

            // This test ensures we don't get NullReferenceException if parent is null
            // (though constructor prevents null parent in reality)
            mockParentViewModel.SetupGet(vm => vm.ShowNotification).Throws<NullReferenceException>();

            // Act & Assert (should not throw)
            timerMock.Raise(t => t.Tick += null, this, eventArgs);
        }

        /// <summary>
        /// Tests that ShowTemporaryNotification doesn't throw when timer start fails.
        /// </summary>
        [Fact]
        public void ShowTemporaryNotification_DoesNotThrow_WhenTimerStartFails()
        {
            // Arrange
            mockTimerService.Setup(t => t.Start()).Throws<InvalidOperationException>();

            // Act & Assert (should not throw)
            notificationHelper.ShowTemporaryNotification("Test");
        }

        /// <summary>
        /// Tests that Dispose properly cleans up event handlers.
        /// </summary>
        [Fact]
        public void Dispose_CleansUpEventHandlers()
        {
            // Arrange
            var timerMock = new Mock<IDispatcherTimerService>();
            var helper = new NotificationHelper(mockParentViewModel.Object, timerMock.Object);

            // Act
            if (helper is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // Assert
            timerMock.VerifyRemove(t => t.Tick -= It.IsAny<EventHandler>(), Times.AtLeastOnce);
        }
    }
}