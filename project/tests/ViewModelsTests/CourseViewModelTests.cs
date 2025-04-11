namespace Tests.ViewModelsTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using CourseApp.Models;
    using CourseApp.Services;
    using CourseApp.Services.Helpers;
    using CourseApp.ViewModels;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="CourseViewModel"/>.
    /// Verifies course enrollment, module completion, timer functionality,
    /// and reward claiming behavior.
    /// </summary>
    public class CourseViewModelTests
    {
        private readonly Mock<ICourseService> mockCourseService;
        private readonly Mock<ICoinsService> mockCoinsService;
        private readonly DispatcherTimerService mockCourseTimer;
        private readonly DispatcherTimerService mockNotificationTimer;
        private readonly Course testCourse;
        private readonly CourseViewModel viewModel;

        /// <summary>
        /// Initializes test dependencies and configures default mock behaviors.
        /// Creates a test course with 3 modules (2 required, 1 bonus).
        /// </summary>
        public CourseViewModelTests()
        {
            mockCourseService = new Mock<ICourseService>();
            mockCoinsService = new Mock<ICoinsService>();
            var mockCourseTimerInterface = new Mock<IDispatcherTimer>();
            mockCourseTimerInterface.SetupProperty(t => t.Interval);
            var mockNotificationTimerInterface = new Mock<IDispatcherTimer>();
            mockNotificationTimerInterface.SetupProperty(t => t.Interval);

            mockCourseTimer = new DispatcherTimerService(mockCourseTimerInterface.Object);
            mockNotificationTimer = new DispatcherTimerService(mockNotificationTimerInterface.Object);


            testCourse = new Course
            {
                CourseId = 1,
                Title = "Test Course",
                Description = "Test Description",
                ImageUrl = "test.jpg",
                Difficulty = "Beginner",
                IsPremium = true,
                Cost = 100,
                TimeToComplete = 3600 // 1 hour
            };

            ConfigureDefaultMocks();
            InitializeTestModules();

            viewModel = new CourseViewModel(
                testCourse,
                mockCourseService.Object,
                mockCoinsService.Object,
                mockCourseTimer,
                mockNotificationTimer);
        }

        private void ConfigureDefaultMocks()
        {
            mockCourseService.Setup(x => x.IsUserEnrolled(It.IsAny<int>())).Returns(false);
            mockCourseService.Setup(x => x.GetTimeSpent(It.IsAny<int>())).Returns(0);
            mockCourseService.Setup(x => x.GetCompletedModulesCount(It.IsAny<int>())).Returns(0);
            mockCourseService.Setup(x => x.GetRequiredModulesCount(It.IsAny<int>())).Returns(5);
            mockCourseService.Setup(x => x.GetCourseTimeLimit(It.IsAny<int>())).Returns(3600);
            mockCoinsService.Setup(x => x.GetCoinBalance(It.IsAny<int>())).Returns(200);
        }

        private void InitializeTestModules()
        {
            var modules = new List<CourseApp.Models.Module>
            {
                new ()
                {
                    ModuleId = 1, Position = 1, IsBonus = false, Cost = 50, Title = "Module 1 Title",
                    Description = "Module 1 Description", ImageUrl = "module1.jpg",
                },
                new ()
                {
                    ModuleId = 2, Position = 2, IsBonus = false, Cost = 50, Title = "Module 2 Title",
                    Description = "Module 2 Description", ImageUrl = "module3.jpg",
                },
                new ()
                {
                    ModuleId = 3, Position = 3, IsBonus = true, Cost = 50, Title = "Module 3 Title",
                    Description = "Module 3 Description", ImageUrl = "module3.jpg",
                },
            };
            mockCourseService.Setup(x => x.GetModules(It.IsAny<int>())).Returns(modules);
        }

        /// <summary>
        /// Verifies that the ViewModel initializes all properties correctly.
        /// </summary>
        [Fact]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            Assert.Equal(testCourse, viewModel.CurrentCourse);
            Assert.False(viewModel.IsEnrolled);
            Assert.Equal(200, viewModel.CoinBalance);
            Assert.Equal("60 min 0 sec", viewModel.FormattedTimeRemaining);
            Assert.Equal(0, viewModel.CompletedModules);
            Assert.Equal(5, viewModel.RequiredModules);
            Assert.False(viewModel.IsCourseCompleted);
            Assert.NotNull(viewModel.EnrollCommand);
            Assert.NotNull(viewModel.ModuleRoadmap);
        }

        /// <summary>
        /// Verifies that EnrollCommand can execute when user has sufficient coins.
        /// </summary>
        [Fact]
        public void EnrollCommand_CanExecute_WhenUserHasEnoughCoins()
        {
            Assert.True(viewModel.EnrollCommand.CanExecute(null));
        }

        /// <summary>
        /// Verifies that EnrollCommand cannot execute when user lacks sufficient coins.
        /// </summary>
        [Fact]
        public void EnrollCommand_CannotExecute_WhenUserDoesNotHaveEnoughCoins()
        {
            mockCoinsService.Setup(x => x.GetCoinBalance(It.IsAny<int>())).Returns(50);
            Assert.False(viewModel.EnrollCommand.CanExecute(null));
        }

        /// <summary>
        /// Verifies successful course enrollment deducts coins and updates enrollment status.
        /// </summary>
        [Fact]
        public void EnrollCommand_ExecutesSuccessfully()
        {
            // Arrange
            mockCoinsService.Setup(x => x.TrySpendingCoins(1, 100)).Returns(true);
            mockCourseService.Setup(x => x.EnrollInCourse(1)).Returns(true);

            // Act
            viewModel.EnrollCommand.Execute(null);

            // Assert
            mockCoinsService.Verify(x => x.TrySpendingCoins(1, 100), Times.Once);
            mockCourseService.Verify(x => x.EnrollInCourse(1), Times.Once);
            Assert.True(viewModel.IsEnrolled);
        }

        /// <summary>
        /// Verifies module completion updates progress and checks for rewards.
        /// </summary>
        [Fact]
        public void MarkModuleAsCompletedAndCheckRewards_UpdatesCompletionStatus()
        {
            // Arrange
            mockCourseService.Setup(x => x.GetCompletedModulesCount(It.IsAny<int>())).Returns(1);

            // Act
            viewModel.MarkModuleAsCompletedAndCheckRewards(1);

            // Assert
            Assert.Equal(1, viewModel.CompletedModules);
            Assert.False(viewModel.IsCourseCompleted);
        }

        /// <summary>
        /// Verifies course completion reward is claimed when all modules are completed.
        /// </summary>
        [Fact]
        public void MarkModuleAsCompletedAndCheckRewards_CompletesCourse_WhenAllModulesDone()
        {
            // Arrange
            mockCourseService.Setup(x => x.GetCompletedModulesCount(It.IsAny<int>())).Returns(5);
            mockCourseService.Setup(x => x.ClaimCompletionReward(It.IsAny<int>())).Returns(true);

            // Act
            viewModel.MarkModuleAsCompletedAndCheckRewards(1);

            // Assert
            Assert.True(viewModel.IsCourseCompleted);
            Assert.True(viewModel.CompletionRewardClaimed);
            mockCourseService.Verify(x => x.ClaimCompletionReward(testCourse.CourseId), Times.Once);
        }

        /// <summary>
        /// Verifies successful bonus module purchase updates UI and shows notification.
        /// </summary>
        [Fact]
        public void AttemptBonusModulePurchase_SuccessfulPurchase_UpdatesUI()
        {
            // Arrange
            var testModule = new CourseApp.Models.Module
            {
                ModuleId = 1,
                IsBonus = true,
                Cost = 50,
                Title = "Bonus Module Title",
                Description = "Bonus Module Description",
                ImageUrl = "bonus-module.jpg",
            };

            mockCoinsService.Setup(x => x.TrySpendingCoins(1, 50)).Returns(true);
            mockCourseService.Setup(x => x.BuyBonusModule(1, 1)).Returns(true);

            // Act
            viewModel.AttemptBonusModulePurchase(testModule);

            // Assert
            Assert.Contains(testModule.Title, viewModel.NotificationMessage);
            Assert.True(viewModel.ShowNotification);
        }

        /// <summary>
        /// Verifies time formatting displays correctly for various durations.
        /// </summary>
        [Theory]
        [InlineData(5, "0 min 5 sec")]
        [InlineData(65, "1 min 5 sec")]
        [InlineData(3600, "60 min 0 sec")]
        [InlineData(3700, "61 min 40 sec")]
        public void FormatTimeRemainingDisplay_FormatsTimeCorrectly(int seconds, string expected)
        {
            var result = CourseViewModel.FormatTimeRemainingDisplay(seconds);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies modules are organized correctly in the roadmap.
        /// </summary>
        [Fact]
        public void LoadAndOrganizeCourseModules_OrganizesModulesCorrectly()
        {
            viewModel.LoadAndOrganizeCourseModules();
            Assert.Equal(3, viewModel.ModuleRoadmap.Count);
            Assert.Equal(1, viewModel.ModuleRoadmap[0].Module.ModuleId);
            Assert.Equal(2, viewModel.ModuleRoadmap[1].Module.ModuleId);
            Assert.Equal(3, viewModel.ModuleRoadmap[2].Module.ModuleId);
        }

        /// <summary>
        /// Verifies timed completion reward is claimed when course is completed within time limit.
        /// </summary>
        [Fact]
        public void CheckForTimedReward_GrantsReward_WhenCompletedWithinTime()
        {
            // Arrange
            mockCourseService.Setup(x => x.GetCompletedModulesCount(It.IsAny<int>())).Returns(5);
            mockCourseService.Setup(x => x.ClaimTimedReward(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            SetPrivateField("totalSecondsSpentOnCourse", 1800); // 30 minutes

            // Act
            viewModel.MarkModuleAsCompletedAndCheckRewards(1);

            // Assert
            Assert.True(viewModel.TimedRewardClaimed);
            mockCourseService.Verify(x => x.ClaimTimedReward(testCourse.CourseId, 1800), Times.Once);
        }

        private void SetPrivateField(string fieldName, object value)
        {
            typeof(CourseViewModel)
                .GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(viewModel, value);
        }
    }
}