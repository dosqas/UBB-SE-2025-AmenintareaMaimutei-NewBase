using Moq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;
using CourseApp.Models;
using CourseApp.Services;
using CourseApp.ViewModels;
using Microsoft.UI.Xaml;

namespace CourseApp.Tests
{
    public class CourseViewModelTests
    {
        private readonly Mock<CourseService> mockCourseService;
        private readonly Mock<CoinsService> mockCoinsService;
        private readonly CourseViewModel viewModel;

        public CourseViewModelTests()
        {
            mockCourseService = new Mock<CourseService>();
            mockCoinsService = new Mock<CoinsService>();
            var course = new Course
            {
                CourseId = 1,
                Cost = 100,
                IsPremium = true,
                TimeToComplete = 500
            };

            viewModel = new CourseViewModel(course, mockCourseService.Object, mockCoinsService.Object);
        }

        [Fact]
        public void Constructor_InitializesViewModel()
        {
            // Assert the initial state of the ViewModel after construction
            Assert.NotNull(viewModel.DisplayedModules);
            Assert.Equal(0, viewModel.CountOfCompletedModules);
            Assert.Equal(0, viewModel.CountOfRequiredModules);
        }

        [Fact]
        public void EnrollInCourseCommand_ExecutesSuccessfully_WhenUserCanEnroll()
        {
            // Arrange
            mockCoinsService.Setup(cs => cs.GetUserCoins(It.IsAny<int>())).Returns(200);
            mockCourseService.Setup(cs => cs.IsUserEnrolled(It.IsAny<int>())).Returns(false);
            mockCourseService.Setup(cs => cs.EnrollInCourse(It.IsAny<int>())).Returns(true);
            mockCourseService.Setup(cs => cs.GetTimeSpent(It.IsAny<int>())).Returns(0);

            // Act
            viewModel.EnrollInCourseCommand.Execute(null);

            // Assert
            Assert.True(viewModel.IsUserEnrolled);
            Assert.Equal(0, viewModel.UserCoinBalance);
        }

        [Fact]
        public void EnrollInCourseCommand_Fails_WhenUserCannotAfford()
        {
            // Arrange
            mockCoinsService.Setup(cs => cs.GetUserCoins(It.IsAny<int>())).Returns(50);
            mockCourseService.Setup(cs => cs.IsUserEnrolled(It.IsAny<int>())).Returns(false);

            // Act
            viewModel.EnrollInCourseCommand.Execute(null);

            // Assert
            Assert.False(viewModel.IsUserEnrolled);
        }

        [Fact]
        public void RefreshDisplayedModules_RefreshesModulesList()
        {
            // Arrange
            var modules = new List<Module>
            {
                new Module { ModuleId = 1, Title = "Module 1", IsBonus = false },
                new Module { ModuleId = 2, Title = "Module 2", IsBonus = false }
            };

            mockCourseService.Setup(cs => cs.GetModules(It.IsAny<int>())).Returns(modules);

            // Act
            viewModel.RefreshDisplayedModules();

            // Assert
            Assert.Equal(2, viewModel.DisplayedModules.Count);
            Assert.Equal("Module 1", viewModel.DisplayedModules[0].Module?.Title);
            Assert.Equal("Module 2", viewModel.DisplayedModules[1].Module?.Title);
        }

        [Fact]
        public void StartProgressTracking_UpdatesProgress()
        {
            // Arrange
            mockCourseService.Setup(cs => cs.IsUserEnrolled(It.IsAny<int>())).Returns(true);
            mockCourseService.Setup(cs => cs.GetTimeSpent(It.IsAny<int>())).Returns(10);
            viewModel.StartProgressTracking();

            // Act
            typeof(CourseViewModel).GetMethod("IncrementCourseProgressTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(viewModel, null);

            // Assert
            Assert.Equal(11, viewModel.UserCoinBalance);
        }

        [Fact]
        public void MarkModuleAsCompleted_UpdatesCompletionStatus()
        {
            // Arrange
            var moduleId = 1;
            mockCourseService.Setup(cs => cs.CompleteModule(It.IsAny<int>(), It.IsAny<int>())).Verifiable();
            mockCourseService.Setup(cs => cs.GetCompletedModulesCount(It.IsAny<int>())).Returns(1);

            // Act
            viewModel.MarkModuleAsCompleted(moduleId);

            // Assert
            mockCourseService.Verify(cs => cs.CompleteModule(moduleId, viewModel.CurrentCourse.CourseId), Times.Once);
            Assert.Equal(1, viewModel.CountOfCompletedModules);
        }

        [Fact]
        public void AttemptBonusModulePurchase_PurchasingBonusModule_UnlocksIt()
        {
            // Arrange
            var module = new Module { ModuleId = 1, Title = "Bonus Module", Cost = 50 };
            mockCoinsService.Setup(cs => cs.GetUserCoins(It.IsAny<int>())).Returns(100);
            mockCourseService.Setup(cs => cs.IsModuleCompleted(It.IsAny<int>())).Returns(false);
            mockCourseService.Setup(cs => cs.BuyBonusModule(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            // Act
            viewModel.AttemptBonusModulePurchase(module);

            // Assert
            Assert.Contains(viewModel.DisplayedModules, dm => dm.Module?.ModuleId == module.ModuleId && dm.IsUnlocked);
        }

        [Fact]
        public void ShowTemporaryNotification_DisplaysMessageAndHidesIt()
        {
            // Arrange
            var message = "Test Notification";
            typeof(CourseViewModel).GetMethod("ShowTemporaryNotification", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(viewModel, new object[] { message });

            // Act
            Assert.True(viewModel.ShouldShowNotification);
            Assert.Equal(message, viewModel.UserNotificationMessage);

            // Simulate the timer expiration
            var hideNotificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            hideNotificationTimer.Tick += (sender, args) =>
            {
                typeof(CourseViewModel).GetProperty("ShouldShowNotification", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(viewModel, false);
            };
            hideNotificationTimer.Start();

            // Assert the notification hides
            hideNotificationTimer.Tick += (sender, args) =>
            {
                Assert.False(viewModel.ShouldShowNotification);
            };
        }
    }
}
