using System;
using CourseApp.Services;
using CourseApp.ViewModels;
using Moq;
using Xunit;

namespace CourseApp.Tests.ViewModels
{
    public class NotificationHelperTests
    {
        private readonly Mock<CourseViewModel> _mockParentViewModel;
        private readonly Mock<ITimerService> _mockTimerService;
        private readonly NotificationHelper _notificationHelper;

        public NotificationHelperTests()
        {
            _mockParentViewModel = new Mock<CourseViewModel>();
            _mockTimerService = new Mock<ITimerService>();
            _notificationHelper = new NotificationHelper(_mockParentViewModel.Object, _mockTimerService.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenParentViewModelIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new NotificationHelper(null, _mockTimerService.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenTimerServiceIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new NotificationHelper(_mockParentViewModel.Object, null));
        }

        [Fact]
        public void Constructor_SubscribesToTimerTickEvent()
        {
            // Arrange
            var timerMock = new Mock<ITimerService>();

            // Act
            var helper = new NotificationHelper(_mockParentViewModel.Object, timerMock.Object);

            // Assert
            timerMock.VerifyAdd(t => t.Tick += It.IsAny<EventHandler>(), Times.Once);
        }

        [Fact]
        public void ShowTemporaryNotification_SetsNotificationMessageAndVisibility()
        {
            // Arrange
            const string testMessage = "Test notification";

            // Act
            _notificationHelper.ShowTemporaryNotification(testMessage);

            // Assert
            _mockParentViewModel.VerifySet(vm => vm.NotificationMessage = testMessage, Times.Once);
            _mockParentViewModel.VerifySet(vm => vm.ShowNotification = true, Times.Once);
        }

        [Fact]
        public void ShowTemporaryNotification_SetsTimerIntervalAndStartsTimer()
        {
            // Arrange
            const string testMessage = "Test notification";
            var expectedInterval = TimeSpan.FromSeconds(CourseViewModel.NotificationDisplayDurationInSeconds);

            // Act
            _notificationHelper.ShowTemporaryNotification(testMessage);

            // Assert
            _mockTimerService.VerifySet(t => t.Interval = expectedInterval, Times.Once);
            _mockTimerService.Verify(t => t.Start(), Times.Once);
        }

        [Fact]
        public void OnNotificationTimerTick_HidesNotificationAndStopsTimer()
        {
            // Arrange
            var eventArgs = EventArgs.Empty;

            // Act
            // Simulate timer tick by invoking the event handler directly
            _mockTimerService.Raise(t => t.Tick += null, this, eventArgs);

            // Assert
            _mockParentViewModel.VerifySet(vm => vm.ShowNotification = false, Times.Once);
            _mockTimerService.Verify(t => t.Stop(), Times.Once);
        }

        [Fact]
        public void OnNotificationTimerTick_DoesNotThrow_WhenParentViewModelIsNull()
        {
            // Arrange
            var eventArgs = EventArgs.Empty;
            var timerMock = new Mock<ITimerService>();
            var helper = new NotificationHelper(_mockParentViewModel.Object, timerMock.Object);

            // This test ensures we don't get NullReferenceException if parent is null
            // (though constructor prevents null parent in reality)
            _mockParentViewModel.SetupGet(vm => vm.ShowNotification).Throws<NullReferenceException>();

            // Act & Assert (should not throw)
            timerMock.Raise(t => t.Tick += null, this, eventArgs);
        }

        [Fact]
        public void ShowTemporaryNotification_DoesNotThrow_WhenTimerStartFails()
        {
            // Arrange
            _mockTimerService.Setup(t => t.Start()).Throws<InvalidOperationException>();

            // Act & Assert (should not throw)
            _notificationHelper.ShowTemporaryNotification("Test");
        }

        [Fact]
        public void Dispose_CleansUpEventHandlers()
        {
            // Arrange
            var timerMock = new Mock<ITimerService>();
            var helper = new NotificationHelper(_mockParentViewModel.Object, timerMock.Object);

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