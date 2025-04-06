//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using Xunit;
//using Moq;
//using CourseApp.Models;
//using CourseApp.Services;
//using CourseApp.ViewModels;

//public class CourseViewModelTests
//{
//    private readonly Mock<CourseService> _mockCourseService;
//    private readonly Mock<CoinsService> _mockCoinsService;
//    private readonly Course _testCourse;
//    private readonly CourseViewModel _viewModel;

//    public CourseViewModelTests()
//    {
//        _mockCourseService = new Mock<CourseService>();
//        _mockCoinsService = new Mock<CoinsService>();

//        _testCourse = new Course
//        {
//            CourseId = 1,
//            Title = "Test Course",
//            IsPremium = true,
//            Cost = 100,
//            TimeToComplete = 3600 // 1 hour
//        };

//        // Setup default mock behaviors
//        _mockCourseService.Setup(x => x.IsUserEnrolled(It.IsAny<int>())).Returns(false);
//        _mockCourseService.Setup(x => x.GetTimeSpent(It.IsAny<int>())).Returns(0);
//        _mockCourseService.Setup(x => x.GetCompletedModulesCount(It.IsAny<int>())).Returns(0);
//        _mockCourseService.Setup(x => x.GetRequiredModulesCount(It.IsAny<int>())).Returns(5);
//        _mockCourseService.Setup(x => x.GetCourseTimeLimit(It.IsAny<int>())).Returns(3600);
//        _mockCoinsService.Setup(x => x.GetUserCoins(It.IsAny<int>())).Returns(200);

//        _viewModel = new CourseViewModel(_testCourse, _mockCourseService.Object, _mockCoinsService.Object);
//    }

//    [Fact]
//    public void Constructor_InitializesPropertiesCorrectly()
//    {
//        // Assert
//        Assert.Equal(_testCourse, _viewModel.CurrentCourse);
//        Assert.False(_viewModel.IsEnrolled);
//        Assert.Equal(200, _viewModel.CoinBalance);
//        Assert.Equal("60 min 0 sec", _viewModel.FormattedTimeRemaining);
//        Assert.Equal(0, _viewModel.CompletedModules);
//        Assert.Equal(5, _viewModel.RequiredModules);
//        Assert.False(_viewModel.IsCourseCompleted);
//    }

//    [Fact]
//    public void EnrollCommand_CanExecute_WhenUserHasEnoughCoins()
//    {
//        // Act & Assert
//        Assert.True(_viewModel.EnrollCommand.CanExecute(null));
//    }

//    [Fact]
//    public void EnrollCommand_CannotExecute_WhenUserDoesNotHaveEnoughCoins()
//    {
//        // Arrange
//        _mockCoinsService.Setup(x => x.GetUserCoins(It.IsAny<int>())).Returns(50);

//        // Act & Assert
//        Assert.False(_viewModel.EnrollCommand.CanExecute(null));
//    }

//    [Fact]
//    public void EnrollCommand_ExecutesSuccessfully()
//    {
//        // Arrange
//        _mockCourseService.Setup(x => x.EnrollInCourse(It.IsAny<int>())).Returns(true);

//        // Act
//        _viewModel.EnrollCommand.Execute(null);

//        // Assert
//        Assert.True(_viewModel.IsEnrolled);
//        _mockCourseService.Verify(x => x.EnrollInCourse(_testCourse.CourseId), Times.Once);
//    }

//    [Fact]
//    public void StartCourseProgressTimer_StartsTimer_WhenEnrolled()
//    {
//        // Arrange
//        typeof(CourseViewModel).GetProperty("IsEnrolled")?.SetValue(_viewModel, true);

//        // Act
//        _viewModel.StartCourseProgressTimer();

//        // Assert
//        Assert.True((bool)typeof(CourseViewModel).GetProperty("IsCourseTimerRunning", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_viewModel));
//    }

//    [Fact]
//    public void PauseCourseProgressTimer_StopsTimer_WhenRunning()
//    {
//        // Arrange
//        typeof(CourseViewModel).GetProperty("IsEnrolled")?.SetValue(_viewModel, true);
//        _viewModel.StartCourseProgressTimer();

//        // Act
//        _viewModel.PauseCourseProgressTimer();

//        // Assert
//        Assert.False((bool)typeof(CourseViewModel).GetProperty("IsCourseTimerRunning", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_viewModel));
//    }

//    [Fact]
//    public void MarkModuleAsCompletedAndCheckRewards_UpdatesCompletionStatus()
//    {
//        // Arrange
//        _mockCourseService.Setup(x => x.GetCompletedModulesCount(It.IsAny<int>())).Returns(4);

//        // Act
//        _viewModel.MarkModuleAsCompletedAndCheckRewards(1);

//        // Assert
//        Assert.Equal(4, _viewModel.CompletedModules);
//        Assert.False(_viewModel.IsCourseCompleted);
//    }

//    [Fact]
//    public void MarkModuleAsCompletedAndCheckRewards_CompletesCourse_WhenAllModulesDone()
//    {
//        // Arrange
//        _mockCourseService.Setup(x => x.GetCompletedModulesCount(It.IsAny<int>())).Returns(5);
//        _mockCourseService.Setup(x => x.ClaimCompletionReward(It.IsAny<int>())).Returns(true);

//        // Act
//        _viewModel.MarkModuleAsCompletedAndCheckRewards(1);

//        // Assert
//        Assert.True(_viewModel.IsCourseCompleted);
//        Assert.True(_viewModel.CompletionRewardClaimed);
//        _mockCourseService.Verify(x => x.ClaimCompletionReward(_testCourse.CourseId), Times.Once);
//    }

//    [Fact]
//    public void AttemptBonusModulePurchase_SuccessfulPurchase_UpdatesUI()
//    {
//        // Arrange
//        var testModule = new Module { ModuleId = 1, Title = "Bonus", IsBonus = true, Cost = 50 };
//        _mockCourseService.Setup(x => x.BuyBonusModule(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

//        // Act
//        _viewModel.AttemptBonusModulePurchase(testModule);

//        // Assert
//        Assert.Equal($"Congratulations! You have purchased bonus module {testModule.Title}, {testModule.Cost} coins have been deducted from your balance.",
//            _viewModel.NotificationMessage);
//        Assert.True(_viewModel.ShowNotification);
//    }

//    [Theory]
//    [InlineData(5, "0 min 5 sec")]
//    [InlineData(65, "1 min 5 sec")]
//    [InlineData(3600, "60 min 0 sec")]
//    [InlineData(3700, "61 min 40 sec")]
//    public void FormatTimeRemainingDisplay_FormatsTimeCorrectly(int seconds, string expected)
//    {
//        // Act & Assert
//        Assert.Equal(expected, typeof(CourseViewModel).GetMethod("FormatTimeRemainingDisplay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.Invoke(null, new object[] { seconds }));
//    }

//    [Theory]
//    [InlineData(true, false, true)] // Premium, not enrolled
//    [InlineData(false, false, false)] // Free, not enrolled
//    [InlineData(true, true, false)] // Premium, enrolled
//    public void CoinVisibility_ReturnsCorrectValue(bool isPremium, bool isEnrolled, bool expected)
//    {
//        // Arrange
//        _testCourse.IsPremium = isPremium;
//        typeof(CourseViewModel).GetProperty("IsEnrolled")?.SetValue(_viewModel, isEnrolled);

//        // Act & Assert
//        Assert.Equal(expected, _viewModel.CoinVisibility);
//    }

//    [Fact]
//    public void TimeRemaining_Decreases_AfterTimerStarts()
//    {
//        // Arrange
//        typeof(CourseViewModel).GetProperty("IsEnrolled")?.SetValue(_viewModel, true);
//        var initialTime = _viewModel.TimeRemaining;

//        // Act
//        _viewModel.StartCourseProgressTimer();
//        // Simulate timer tick
//        _viewModel.PauseCourseProgressTimer();

//        // Assert
//        Assert.True(_viewModel.TimeRemaining < initialTime);
//    }

//    [Fact]
//    public void LoadAndOrganizeCourseModules_OrganizesModulesCorrectly()
//    {
//        // Arrange
//        var modules = new List<Module>
//        {
//            new Module { ModuleId = 1, Position = 1, IsBonus = false },
//            new Module { ModuleId = 2, Position = 2, IsBonus = false },
//            new Module { ModuleId = 3, Position = 3, IsBonus = true }
//        };

//        _mockCourseService.Setup(x => x.GetModules(It.IsAny<int>())).Returns(modules);

//        // Act
//        _viewModel.LoadAndOrganizeCourseModules();

//        // Assert
//        Assert.Equal(3, _viewModel.ModuleRoadmap.Count);
//        Assert.Equal(1, _viewModel.ModuleRoadmap[0].Module.ModuleId);
//        Assert.Equal(2, _viewModel.ModuleRoadmap[1].Module.ModuleId);
//        Assert.Equal(3, _viewModel.ModuleRoadmap[2].Module.ModuleId);
//    }

//    [Fact]
//    public void AttemptBonusModulePurchase_ShowsNotification_WhenPurchaseFails()
//    {
//        // Arrange
//        var testModule = new Module { ModuleId = 1, Title = "Bonus", IsBonus = true, Cost = 50 };
//        _mockCourseService.Setup(x => x.BuyBonusModule(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

//        // Act
//        _viewModel.AttemptBonusModulePurchase(testModule);

//        // Assert
//        Assert.Equal("You do not have enough coins to buy this module.", _viewModel.NotificationMessage);
//        Assert.True(_viewModel.ShowNotification);
//    }
//}