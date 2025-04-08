namespace Tests.ViewModelsTests
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    using Moq;
    using CourseApp.Models;
    using CourseApp.Services;
    using CourseApp.ViewModels;
    using static CourseApp.ViewModels.CourseViewModel;
    using System.Reflection;

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

    public class CourseViewModelTests
    {
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly Mock<ICoinsService> _mockCoinsService;
        private readonly MockTimerService _mockCourseTimer;
        private readonly MockTimerService _mockNotificationTimer;
        private readonly Course _testCourse;
        private readonly CourseViewModel _viewModel;

        public CourseViewModelTests()
        {
            _mockCourseService = new Mock<ICourseService>();
            _mockCoinsService = new Mock<ICoinsService>();
            _mockCourseTimer = new MockTimerService();
            _mockNotificationTimer = new MockTimerService();

            _testCourse = new Course
            {
                CourseId = 1,
                Title = "Test Course",
                Description = "Test Description",
                ImageUrl = "Test Image URL",
                Difficulty = "Test Difficulty",
                IsPremium = true,
                Cost = 100,
                TimeToComplete = 3600 // 1 hour
            };

            // Setup default mock behaviors
            _mockCourseService.Setup(x => x.IsUserEnrolled(It.IsAny<int>())).Returns(false);
            _mockCourseService.Setup(x => x.GetTimeSpent(It.IsAny<int>())).Returns(0);
            _mockCourseService.Setup(x => x.GetCompletedModulesCount(It.IsAny<int>())).Returns(0);
            _mockCourseService.Setup(x => x.GetRequiredModulesCount(It.IsAny<int>())).Returns(5);
            _mockCourseService.Setup(x => x.GetCourseTimeLimit(It.IsAny<int>())).Returns(3600);
            _mockCoinsService.Setup(x => x.GetCoinBalance(It.IsAny<int>())).Returns(200);

            // Setup modules
            var modules = new List<CourseApp.Models.Module>
            {
                new CourseApp.Models.Module { ModuleId = 1, Position = 1, IsBonus = false, Title = "Module 1", Description = "Description 1", ImageUrl = "ImageUrl 1" },
                new CourseApp.Models.Module { ModuleId = 2, Position = 2, IsBonus = false, Title = "Module 2", Description = "Description 2", ImageUrl = "ImageUrl 2" },
                new CourseApp.Models.Module { ModuleId = 3, Position = 3, IsBonus = true, Title = "Module 3", Description = "Description 3", ImageUrl = "ImageUrl 3" }
            };
            _mockCourseService.Setup(x => x.GetModules(It.IsAny<int>())).Returns(modules);

            _viewModel = new CourseViewModel(
                _testCourse,
                _mockCourseService.Object,
                _mockCoinsService.Object,
                _mockCourseTimer,
                _mockNotificationTimer);
        }

        [Fact]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Assert
            Assert.Equal(_testCourse, _viewModel.CurrentCourse);
            Assert.False(_viewModel.IsEnrolled);
            Assert.Equal(200, _viewModel.CoinBalance);
            Assert.Equal("60 min 0 sec", _viewModel.FormattedTimeRemaining);
            Assert.Equal(0, _viewModel.CompletedModules);
            Assert.Equal(5, _viewModel.RequiredModules);
            Assert.False(_viewModel.IsCourseCompleted);
            Assert.NotNull(_viewModel.EnrollCommand);
            Assert.NotNull(_viewModel.ModuleRoadmap);
        }

        [Fact]
        public void EnrollCommand_CanExecute_WhenUserHasEnoughCoins()
        {
            // Act & Assert
            Assert.True(_viewModel.EnrollCommand.CanExecute(null));
        }

        [Fact]
        public void EnrollCommand_CannotExecute_WhenUserDoesNotHaveEnoughCoins()
        {
            // Arrange
            _mockCoinsService.Setup(x => x.GetCoinBalance(It.IsAny<int>())).Returns(50);

            // Act & Assert
            Assert.False(_viewModel.EnrollCommand.CanExecute(null));
        }

        [Fact]
        public void EnrollCommand_ExecutesSuccessfully()
        {
            // Arrange
            var testModule = new CourseApp.Models.Module
            {
                ModuleId = 1,
                Title = "Bonus",
                IsBonus = true,
                Cost = 50,
                Description = "Bonus Module Description",
                ImageUrl = "Bonus Image URL"
            };
            var newCourse = new Course
            {
                CourseId = 1,
                Title = "Test Course",
                Description = "Test Description",
                ImageUrl = "Test Image URL",
                Difficulty = "Test Difficulty",
                IsPremium = true,
                Cost = 100,
                TimeToComplete = 3600 // 1 hour
            };

            // Use reflection to set the private property
            typeof(CourseViewModel).GetProperty("CurrentCourse", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_viewModel, newCourse);

            _mockCoinsService.Setup(x => x.TrySpendingCoins(1, 100)).Returns(true);
            _mockCourseService.Setup(x => x.EnrollInCourse(1)).Returns(true);

            // Act
            _viewModel.EnrollCommand.Execute(testModule);

            // Assert
            _mockCoinsService.Verify(x => x.TrySpendingCoins(1, 100), Times.Once);
        }


        [Fact]
        public void StartCourseProgressTimer_StartsTimer_WhenEnrolled()
        {
            // Arrange
            _mockCourseService.Setup(x => x.IsUserEnrolled(It.IsAny<int>())).Returns(true);
            _viewModel.EnrollCommand.Execute(null); // Enroll first

            // Act
            _viewModel.StartCourseProgressTimer();

            // Assert
            Assert.True(_mockCourseTimer.IsRunning);
        }

        [Fact]
        public void PauseCourseProgressTimer_SavesProgress_WhenTimerWasRunning()
        {
            // Arrange
            _mockCourseService.Setup(x => x.IsUserEnrolled(It.IsAny<int>())).Returns(true);
            _viewModel.EnrollCommand.Execute(null);
            _viewModel.StartCourseProgressTimer();

            // Act
            _viewModel.PauseCourseProgressTimer();

            // Assert
            Assert.False(_mockCourseTimer.IsRunning);
            _mockCourseService.Verify(x => x.UpdateTimeSpent(
                _testCourse.CourseId,
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void MarkModuleAsCompletedAndCheckRewards_UpdatesCompletionStatus()
        {
            // Arrange
            _mockCourseService.SetupSequence(x => x.GetCompletedModulesCount(It.IsAny<int>()))
                .Returns(1)  // After first completion
                .Returns(1); // For property change notification

            // Act
            _viewModel.MarkModuleAsCompletedAndCheckRewards(1);

            // Assert
            Assert.Equal(1, _viewModel.CompletedModules);
            Assert.False(_viewModel.IsCourseCompleted);
        }

        [Fact]
        public void MarkModuleAsCompletedAndCheckRewards_CompletesCourse_WhenAllModulesDone()
        {
            // Arrange
            _mockCourseService.Setup(x => x.GetCompletedModulesCount(It.IsAny<int>())).Returns(5);
            _mockCourseService.Setup(x => x.ClaimCompletionReward(It.IsAny<int>())).Returns(true);

            // Act
            _viewModel.MarkModuleAsCompletedAndCheckRewards(1);

            // Assert
            Assert.True(_viewModel.IsCourseCompleted);
            Assert.True(_viewModel.CompletionRewardClaimed);
            _mockCourseService.Verify(x => x.ClaimCompletionReward(_testCourse.CourseId), Times.Once);
        }

        [Fact]
        public void AttemptBonusModulePurchase_SuccessfulPurchase_UpdatesUI()
        {
            // Arrange
            var testModule = new CourseApp.Models.Module
            {
                ModuleId = 1,
                Title = "Bonus",
                IsBonus = true,
                Cost = 50,
                Description = "Bonus Module Description",
                ImageUrl = "Bonus Image URL"
            };

            var testCourse = new Course
            {
                CourseId = 1,
                Title = "Test Course",
                Description = "Test Description",
                ImageUrl = "Test Image URL",
                Difficulty = "Test Difficulty",
                IsPremium = true,
                Cost = 100,
                TimeToComplete = 3600 // 1 hour
            };

            _mockCoinsService
                .Setup(x => x.TrySpendingCoins(1, 50))
                .Returns(true);

            _mockCourseService
                .Setup(x => x.BuyBonusModule(1, 1))
                .Returns(true);

            // Act
            _viewModel.AttemptBonusModulePurchase(testModule);

            // Assert
            Assert.Equal(
                $"Congratulations! You have purchased bonus module {testModule.Title}, {testModule.Cost} coins have been deducted from your balance.",
                _viewModel.NotificationMessage);

            Assert.True(_viewModel.ShowNotification);

            _mockCoinsService.Verify(
                x => x.TrySpendingCoins(1, testModule.Cost),
                Times.Once);
        }


        [Theory]
        [InlineData(5, "0 min 5 sec")]
        [InlineData(65, "1 min 5 sec")]
        [InlineData(3600, "60 min 0 sec")]
        [InlineData(3700, "61 min 40 sec")]
        public void FormatTimeRemainingDisplay_FormatsTimeCorrectly(int seconds, string expected)
        {
            // Act
            var result = CourseViewModel.FormatTimeRemainingDisplay(seconds);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void LoadAndOrganizeCourseModules_OrganizesModulesCorrectly()
        {
            // Act
            _viewModel.LoadAndOrganizeCourseModules();

            // Assert
            Assert.Equal(3, _viewModel.ModuleRoadmap.Count);
            Assert.Equal(1, _viewModel.ModuleRoadmap[0].Module.ModuleId);
            Assert.Equal(2, _viewModel.ModuleRoadmap[1].Module.ModuleId);
            Assert.Equal(3, _viewModel.ModuleRoadmap[2].Module.ModuleId);
        }

        [Fact]
        public void AttemptBonusModulePurchase_ShowsNotification_WhenPurchaseFails()
        {
            // Arrange
            var testModule = new CourseApp.Models.Module { ModuleId = 1, Title = "Bonus", IsBonus = true, Cost = 50, Description = "Bonus Module Description", ImageUrl = "Bonus Image URL" };
            _mockCourseService.Setup(x => x.BuyBonusModule(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            _mockCoinsService.Setup(x => x.GetCoinBalance(It.IsAny<int>())).Returns(200);

            // Act
            _viewModel.AttemptBonusModulePurchase(testModule);

            // Assert
            Assert.Equal("You do not have enough coins to buy this module.", _viewModel.NotificationMessage);
            Assert.True(_viewModel.ShowNotification);
        }

        [Fact]
        public void CheckForTimedReward_GrantsReward_WhenCompletedWithinTime()
        {
            // Arrange
            _mockCourseService.Setup(x => x.GetCompletedModulesCount(It.IsAny<int>())).Returns(5);
            _mockCourseService.Setup(x => x.ClaimTimedReward(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            // Simulate time spent (30 minutes)
            typeof(CourseViewModel).GetField("totalSecondsSpentOnCourse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_viewModel, 1800);

            // Act
            _viewModel.MarkModuleAsCompletedAndCheckRewards(1);

            // Assert
            Assert.True(_viewModel.TimedRewardClaimed);
            _mockCourseService.Verify(x => x.ClaimTimedReward(_testCourse.CourseId, 1800), Times.Once);
        }

/*        [Fact]
        public void NotificationHelper_HidesNotification_AfterTimeout()
        {
            // Arrange
            var testMessage = "Test notification";
            _viewModel.NotificationMessage = testMessage;
            _viewModel.ShowNotification = true;

            // Act - Simulate timer tick
            _mockNotificationTimer.SimulateTick();

            // Assert
            Assert.False(_viewModel.ShowNotification);
        }*/
    }
}
