namespace Tests.ViewModelsTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using CourseApp.Models;
    using CourseApp.Services;
    using CourseApp.Services.Helpers;
    using CourseApp.ViewModels;
    using CourseApp.ViewModels.Helpers;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="CourseViewModel"/>.
    /// Verifies course enrollment, module completion, timer functionality,
    /// and reward claiming behavior.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CourseViewModelTests
    {
        private readonly Mock<ICourseService> mockCourseService;
        private readonly Mock<ICoinsService> mockCoinsService;
        private readonly Mock<IDispatcherTimerService> mockCourseTimer;
        private readonly Mock<IDispatcherTimerService> mockNotificationTimer;
        private readonly Mock<INotificationHelper> mockNotificationHelper;
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

            mockCourseTimer = new Mock<IDispatcherTimerService>();
            mockNotificationTimer = new Mock<IDispatcherTimerService>();

            mockNotificationHelper = new Mock<INotificationHelper>();

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
                mockCourseTimer.Object,
                mockNotificationTimer.Object,
                mockNotificationHelper.Object);
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
            mockCoinsService.Setup(x => x.TrySpendingCoins(0, 100)).Returns(true);
            mockCourseService.Setup(x => x.EnrollInCourse(1)).Returns(true);

            // Act
            viewModel.EnrollCommand.Execute(null);

            // Assert
            mockCoinsService.Verify(x => x.TrySpendingCoins(0, 100), Times.Once);
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

            mockCoinsService.Setup(x => x.TrySpendingCoins(0, 50)).Returns(true);
            mockCourseService.Setup(x => x.BuyBonusModule(1, 1)).Returns(true);
            mockNotificationHelper.Setup(helper => helper.ShowTemporaryNotification(It.IsAny<string>())).Verifiable();

            // Act
            viewModel.AttemptBonusModulePurchase(testModule);

            // Assert
            mockNotificationHelper.Verify(helper => helper.ShowTemporaryNotification(It.Is<string>(msg => msg.Contains("Congratulations! You have purchased bonus module"))), Times.Once);  // Check if the correct message was passed
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

        [Fact]
        public void AttemptBonusModulePurchase_ThrowsArgumentNullException_WhenModuleIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => viewModel.AttemptBonusModulePurchase(null));
        }

        [Fact]
        public void AttemptBonusModulePurchase_ReturnsEarly_WhenModuleIsCompleted()
        {
            // Arrange
            var completedModule = new CourseApp.Models.Module
            {
                ModuleId = 1,
                IsBonus = true,
                Cost = 50,
                Title = "Module Title",
                Description = "Module Description",
                ImageUrl = "module.jpg",
            };
            mockCourseService.Setup(service => service.IsModuleCompleted(completedModule.ModuleId)).Returns(true);
            mockNotificationHelper.Setup(helper => helper.ShowTemporaryNotification(It.IsAny<string>())).Verifiable();

            // Act
            viewModel.AttemptBonusModulePurchase(completedModule);

            // Assert
            mockCoinsService.Verify(service => service.TrySpendingCoins(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            mockCourseService.Verify(service => service.BuyBonusModule(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            mockNotificationHelper.Verify(helper => helper.ShowTemporaryNotification("You do not have enough coins to buy this module."), Times.Never);
        }

        [Fact]
        public void AttemptBonusModulePurchase_ShowsPurchaseFailedNotification_WhenNotEnoughCoins()
        {
            // Arrange
            var module = new CourseApp.Models.Module
            {
                ModuleId = 3,
                IsBonus = true,
                Cost = 200,
                Title = "Module Title",
                Description = "Module Description",
                ImageUrl = "module.jpg",
            };

            // Mock dependencies
            mockCoinsService.Setup(service => service.TrySpendingCoins(0, module.Cost)).Returns(false); // Simulate insufficient coins
            mockCourseService.Setup(service => service.IsModuleCompleted(It.IsAny<int>())).Returns(false); // Simulate module not completed
            mockNotificationHelper.Setup(helper => helper.ShowTemporaryNotification(It.IsAny<string>())).Verifiable(); // Setup mock for ShowTemporaryNotification

            // Act
            viewModel.AttemptBonusModulePurchase(module);

            // Verify that the method is invoked once
            mockCoinsService.Verify(service => service.TrySpendingCoins(0, module.Cost), Times.Once);

            // Assert: Verify the notification helper is called with the expected message
            mockNotificationHelper.Verify(helper => helper.ShowTemporaryNotification("You do not have enough coins to buy this module."), Times.Once);
        }

        [Fact]
        public void AttemptBonusModulePurchase_SuccessfulPurchase_UpdatesStatusAndShowsNotification()
        {
            // Arrange
            var module = new CourseApp.Models.Module
            {
                ModuleId = 3,
                IsBonus = true,
                Cost = 200,
                Title = "Module Title",
                Description = "Module Description",
                ImageUrl = "module.jpg",
            };
            mockCoinsService.Setup(service => service.TrySpendingCoins(It.IsAny<int>(), module.Cost)).Returns(true);
            mockCourseService.Setup(service => service.BuyBonusModule(module.ModuleId, testCourse.CourseId)).Returns(true);
            mockNotificationHelper.Setup(helper => helper.ShowTemporaryNotification(It.IsAny<string>())).Verifiable();

            // Act
            viewModel.AttemptBonusModulePurchase(module);

            // Assert
            mockCoinsService.Verify(service => service.TrySpendingCoins(It.IsAny<int>(), module.Cost), Times.Once);
            mockCourseService.Verify(service => service.BuyBonusModule(module.ModuleId, testCourse.CourseId), Times.Once);
            mockCourseService.Verify(service => service.OpenModule(module.ModuleId), Times.Once);
            mockNotificationHelper.Verify(helper =>
            helper.ShowTemporaryNotification($"Congratulations! You have purchased bonus module {module.Title}, {module.Cost} coins have been deducted from your balance."), Times.Once);
        }

        [Fact]
        public void AttemptBonusModulePurchase_FailedPurchase_ShowsPurchaseFailedNotification()
        {
            // Arrange
            var module = new CourseApp.Models.Module
            {
                ModuleId = 3,
                IsBonus = true,
                Cost = 50,
                Title = "Module Title",
                Description = "Module Description",
                ImageUrl = "module.jpg",
            };
            mockCoinsService.Setup(service => service.TrySpendingCoins(It.IsAny<int>(), module.Cost)).Returns(true);
            mockCourseService.Setup(service => service.BuyBonusModule(module.ModuleId, testCourse.CourseId)).Returns(false);
            mockNotificationHelper.Setup(helper => helper.ShowTemporaryNotification(It.IsAny<string>())).Verifiable();

            // Act
            viewModel.AttemptBonusModulePurchase(module);

            // Assert
            mockCoinsService.Verify(service => service.TrySpendingCoins(It.IsAny<int>(), module.Cost), Times.Once);
            mockCourseService.Verify(service => service.BuyBonusModule(module.ModuleId, testCourse.CourseId), Times.Once);
            mockNotificationHelper.Verify(helper => helper.ShowTemporaryNotification("You do not have enough coins to buy this module."), Times.Once);
        }

        private void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
            else
            {
                var fieldInfo = obj.GetType().GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
                fieldInfo?.SetValue(obj, value);
            }
        }

        [Fact]
        public void NotificationMessage_SetValue_RaisesPropertyChanged()
        {
            // Arrange
            var wasCalled = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.NotificationMessage))
                    wasCalled = true;
            };

            // Act
            viewModel.NotificationMessage = "Test Message";

            // Assert
            Assert.Equal("Test Message", viewModel.NotificationMessage);
            Assert.True(wasCalled);
        }

        [Fact]
        public void ShowNotification_SetValue_RaisesPropertyChanged()
        {
            // Arrange
            var wasCalled = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.ShowNotification))
                    wasCalled = true;
            };

            // Act
            viewModel.ShowNotification = true;

            // Assert
            Assert.True(viewModel.ShowNotification);
            Assert.True(wasCalled);
        }

        [Fact]
        public void Tags_ReturnsTagsFromService()
        {
            // Arrange
            var expectedTags = new List<Tag>
            {
                new Tag { Name = "Tag1" },
                new Tag { Name = "Tag2" }
            };
            mockCourseService.Setup(s => s.GetCourseTags(testCourse.CourseId)).Returns(expectedTags);

            // Act
            var tags = viewModel.Tags;

            // Assert
            Assert.Equal(2, tags.Count);
            Assert.Equal("Tag1", tags[0].Name);
            Assert.Equal("Tag2", tags[1].Name);
        }

        [Fact]
        public void CoinVisibility_ReturnsTrue_WhenCourseIsPremiumAndUserNotEnrolled()
        {
            // Arrange
            testCourse.IsPremium = true;
            mockCourseService.Setup(s => s.IsUserEnrolled(testCourse.CourseId)).Returns(false);

            var vm = new CourseViewModel(
                testCourse,
                mockCourseService.Object,
                mockCoinsService.Object,
                mockCourseTimer.Object,
                mockNotificationTimer.Object,
                mockNotificationHelper.Object);

            // Act
            var visible = vm.CoinVisibility;

            // Assert
            Assert.True(visible);
        }

        [Fact]
        public void CoinVisibility_ReturnsFalse_WhenUserIsEnrolled()
        {
            // Arrange
            testCourse.IsPremium = true;
            mockCourseService.Setup(s => s.IsUserEnrolled(testCourse.CourseId)).Returns(true);

            var vm = new CourseViewModel(
                testCourse,
                mockCourseService.Object,
                mockCoinsService.Object,
                mockCourseTimer.Object,
                mockNotificationTimer.Object,
                mockNotificationHelper.Object);

            // Act
            var visible = vm.CoinVisibility;

            // Assert
            Assert.False(visible);
        }

        [Fact]
        public void CoinVisibility_ReturnsFalse_WhenCourseIsNotPremium()
        {
            // Arrange
            testCourse.IsPremium = false;
            mockCourseService.Setup(s => s.IsUserEnrolled(testCourse.CourseId)).Returns(false);

            var vm = new CourseViewModel(
                testCourse,
                mockCourseService.Object,
                mockCoinsService.Object,
                mockCourseTimer.Object,
                mockNotificationTimer.Object,
                mockNotificationHelper.Object);

            // Act
            var visible = vm.CoinVisibility;

            // Assert
            Assert.False(visible);
        }


        [Fact]
        public void Constructor_WithNullServices_InitializesDefaults()
        {
            // Arrange
            var course = new Course
            {
                CourseId = 42,
                Title = "Sample Course",
                Description = "Sample Description",
                ImageUrl = "sample.jpg",
                Difficulty = "Intermediate",
                IsPremium = false,
                TimeToComplete = 1800
            };

            // Act
            var exception = Record.Exception(() =>
            {
                var vm = new CourseViewModel(
                    course,
                    null, // courseService
                    null, // coinsService
                    mockCourseTimer.Object, // can't be null since we cannot mock a DispatcherTimer (which it default to if null)
                    mockNotificationTimer.Object, // can't be null since we cannot mock a DispatcherTimer (which it default to if null)
                    null // notificationHelper
                );
            });

            // Assert
            Assert.Null(exception); // Constructor should not throw
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenCourseIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new CourseViewModel(null!)); // using null-forgiving operator to suppress warning

            Assert.Equal("course", exception.ParamName);
        }

        [Fact]
        public void CourseTimer_Tick_IncrementsTimeAndUpdatesFormattedTime()
        {
            // Arrange
            var initialFormattedTime = viewModel.FormattedTimeRemaining;

            // Act
            mockCourseTimer.SimulateTick();

            // Access private field directly using reflection
            var fieldInfo = typeof(CourseViewModel).GetField("totalSecondsSpentOnCourse", BindingFlags.NonPublic | BindingFlags.Instance);
            var totalSecondsSpent = (int?)fieldInfo?.GetValue(viewModel);

            // Assert
            Assert.Equal(1, totalSecondsSpent);
            Assert.NotEqual(initialFormattedTime, viewModel.FormattedTimeRemaining);
        }

        [Fact]
        public void EnrollCommand_ShouldNotExecute_WhenUserIsAlreadyEnrolled()
        {
            // Arrange  
            mockCourseService.Setup(x => x.IsUserEnrolled(It.IsAny<int>())).Returns(true);  // Simulate that the user is enrolled
            var tempViewModel = new CourseViewModel(
                testCourse,
                mockCourseService.Object,
                mockCoinsService.Object,
                mockCourseTimer,
                mockNotificationTimer,
                mockNotificationHelper.Object);

            // Act  
            bool canExecute = tempViewModel.EnrollCommand.CanExecute(null);

            // Assert  
            Assert.False(canExecute, "Enroll command should not be executable when the user is already enrolled.");
        }

        [Fact]
        public void EnrollCommand_ShouldExecute_WhenUserHasEnoughCoins()
        {
            // Arrange
            mockCoinsService.Setup(x => x.GetCoinBalance(It.IsAny<int>())).Returns(200); // User has enough coins

            // Act
            bool canExecute = viewModel.EnrollCommand.CanExecute(null);

            // Assert
            Assert.True(canExecute, "Enroll command should be executable when the user has enough coins.");
        }

        [Fact]
        public void EnrollCommand_ShouldNotExecute_WhenUserDoesNotHaveEnoughCoins()
        {
            // Arrange
            mockCoinsService.Setup(x => x.GetCoinBalance(It.IsAny<int>())).Returns(50); // User has insufficient coins (less than the course cost)
            mockCoinsService.Setup(x => x.TrySpendingCoins(It.IsAny<int>(), It.IsAny<int>())).Returns(false); // Simulate failure to spend coins

            // Act
            bool canExecute = viewModel.EnrollCommand.CanExecute(null); // Check if the command can execute
            viewModel.EnrollCommand.Execute(null); // Try to execute the command

            // Assert
            Assert.False(canExecute, "Enroll command should not be executable when the user does not have enough coins.");

            // Verify that the EnrollInCourse method is not called
            mockCourseService.Verify(x => x.EnrollInCourse(It.IsAny<int>()), Times.Never); // Ensure EnrollInCourse was not called
            mockCoinsService.Verify(x => x.TrySpendingCoins(It.IsAny<int>(), It.IsAny<int>()), Times.Once); // Ensure TrySpendingCoins was called once
            mockCoinsService.Verify(x => x.GetCoinBalance(It.IsAny<int>()), Times.Once); // Ensure GetCoinBalance was called once
        }

        [Fact]
        public void EnrollCommand_ShouldNotExecute_WhenCourseEnrollmentFails()
        {
            // Arrange
            mockCoinsService.Setup(x => x.TrySpendingCoins(It.IsAny<int>(), It.IsAny<int>())).Returns(true); // Simulate successful coin spending
            mockCourseService.Setup(x => x.EnrollInCourse(It.IsAny<int>())).Returns(false); // Simulate enrollment failure

            var viewModel = new CourseViewModel(
                testCourse,
                mockCourseService.Object,
                mockCoinsService.Object,
                mockCourseTimer,
                mockNotificationTimer,
                mockNotificationHelper.Object);

            // Act
            bool canExecute = viewModel.EnrollCommand.CanExecute(null); // Check if the command can execute
            viewModel.EnrollCommand.Execute(null); // Try to execute the command

            // Assert
            Assert.True(canExecute, "Enroll command should be executable when the user has enough coins.");

            mockCourseService.Verify(x => x.EnrollInCourse(It.IsAny<int>()), Times.Once); // Ensure that EnrollInCourse is called
            mockCoinsService.Verify(x => x.TrySpendingCoins(It.IsAny<int>(), It.IsAny<int>()), Times.Once); // Ensure TrySpendingCoins is called

            // Verify that the UI state is not updated (IsEnrolled should remain false)
            Assert.False(viewModel.IsEnrolled, "IsEnrolled should remain false when enrollment fails.");
        }

        [Fact]
        public void StartCourseProgressTimer_ShouldNotStart_WhenUserIsNotEnrolled()
        {
            // Arrange
            mockCourseService.Setup(x => x.IsUserEnrolled(It.IsAny<int>())).Returns(false); // User is not enrolled
            var viewModel = new CourseViewModel(
                testCourse,
                mockCourseService.Object,
                mockCoinsService.Object,
                mockCourseTimer,
                mockNotificationTimer,
                mockNotificationHelper.Object);

            // Act
            viewModel.StartCourseProgressTimer(); // Call the method

            // Assert
            Assert.False(viewModel.IsCourseTimerRunning, "Course progress timer should not start when the user is not enrolled.");
            mockCourseTimer.Verify(x => x.Start(), Times.Never, "The course progress timer should not be started when the user is not enrolled.");
        }

    }
}