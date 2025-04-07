//using Xunit;
//using CourseApp.ViewModels;
//using CourseApp.Models;
//using CourseApp.Services;
//using Moq;

//public class DummyCourseService : ICourseService
//{
//    private readonly List<Course> courses;
//    private readonly List<Module> modules;

//    public DummyCourseService()
//    {
//        // Setting up dummy data for testing purposes.
//        courses = new List<Course>
//            {
//                new Course { CourseId = 1, Title = "Course 1", Description = "Description for Course 1", Difficulty = "Easy", ImageUrl = "course1.png" },
//                new Course { CourseId = 2, Title = "Course 2", Description = "Description for Course 2", Difficulty = "Medium", ImageUrl = "course2.png" }
//            };

//        modules = new List<Module>
//            {
//                new Module { ModuleId = 1, CourseId = 1, Title = "Module 1", Description = "Module 1 Description", Position = 1, IsBonus = false, Cost = 0, ImageUrl = "module1.png" },
//                new Module { ModuleId = 2, CourseId = 1, Title = "Module 2", Description = "Module 2 Description", Position = 2, IsBonus = false, Cost = 0, ImageUrl = "module2.png" },
//                new Module { ModuleId = 3, CourseId = 2, Title = "Module 3", Description = "Module 3 Description", Position = 1, IsBonus = false, Cost = 0, ImageUrl = "module3.png" }
//            };
//    }

//    public List<Course> GetCourses() => courses;

//    public List<Module> GetModules(int courseId) =>
//        modules.FindAll(m => m.CourseId == courseId);

//    public List<Module> GetNormalModules(int courseId) =>
//        modules.FindAll(m => m.CourseId == courseId && !m.IsBonus);

//    public List<Tag> GetTags() => new List<Tag>();  // Return empty for now

//    public List<Tag> GetCourseTags(int courseId) => new List<Tag>();  // Return empty for now

//    public bool IsUserEnrolled(int courseId) => true; // Assume always enrolled

//    public bool EnrollInCourse(int courseId) => true;  // Assume successful enrollment

//    public bool IsModuleCompleted(int moduleId) => false;  // Assume modules are not completed

//    public void CompleteModule(int moduleId, int courseId) { }

//    public void OpenModule(int moduleId) { }

//    public bool BuyBonusModule(int moduleId, int courseId) => false;  // Assume no bonus purchase

//    public List<Course> GetFilteredCourses(string searchText, bool filterPremium, bool filterFree, bool filterEnrolled, bool filterNotEnrolled, List<int> selectedTagIds) =>
//        courses;  // Return all courses

//    public void UpdateTimeSpent(int courseId, int seconds) { }

//    public int GetTimeSpent(int courseId) => 0;  // Return dummy time spent

//    public bool ClickModuleImage(int moduleId) => true;  // Assume clicking the image works

//    public bool IsModuleInProgress(int moduleId) => false;  // Assume no module in progress

//    public bool IsModuleAvailable(int moduleId) => true;  // Assume all modules are available

//    public bool IsCourseCompleted(int courseId) => false;  // Assume no course is completed

//    public int GetCompletedModulesCount(int courseId) => 0;  // Return dummy count

//    public int GetRequiredModulesCount(int courseId) => 2;  // Return dummy required count

//    public bool ClaimCompletionReward(int courseId) => true;  // Assume reward claimed

//    public bool ClaimTimedReward(int courseId, int timeSpent) => true;  // Assume reward claimed

//    public int GetCourseTimeLimit(int courseId) => 1000;  // Return dummy course time limit
//}


//public class DummyCoinsService : ICoinsService
//{
//    public int GetCoinBalance(int userId)
//    {
//        return 200; // Return a static balance for testing
//    }

//    public bool TrySpendingCoins(int userId, int cost)
//    {
//        return true; // Simulate successful spending
//    }

//    public void AddCoins(int userId, int amount)
//    {
//        // Simulate adding coins (no-op for testing)
//    }

//    public bool ApplyDailyLoginBonu(int userId = 0)
//    {
//        return true; // Simulate successful bonus application
//    }
//}

//public class DummyCourseViewModel : CourseViewModel
//{
//    public DummyCourseViewModel() : base(new DummyCourseService(), new DummyCoinsService()) { }
//}

//public class ModuleViewModelTests
//{
//    private Module GetTestModule() => new Module
//    {
//        ModuleId = 1,
//        CourseId = 101,
//        Title = "Test Module",
//        Description = "This is a test description that is quite long.",
//        Position = 1,
//        IsBonus = false,
//        Cost = 0,
//        ImageUrl = "test.png"
//    };

//    [Fact]
//    public void IsCompleted_ShouldBeTrue_WhenServiceReturnsTrue()
//    {
//        // Arrange
//        var module = GetTestModule();
//        var courseService = new DummyCourseService();
//        var coinsService = new DummyCoinsService();
//        var courseVM = new DummyCourseViewModel();

//        // Act
//        var viewModel = new ModuleViewModel(module, courseVM, courseService, coinsService);

//        // Assert
//        Assert.False(viewModel.IsCompleted); // Since DummyCourseService returns false
//    }

//    [Fact]
//    public void CoinBalance_ShouldReturnCorrectBalance()
//    {
//        // Arrange
//        var module = GetTestModule();
//        var courseService = new DummyCourseService();
//        var coinsService = new DummyCoinsService();
//        var courseVM = new DummyCourseViewModel();

//        // Act
//        var viewModel = new ModuleViewModel(module, courseVM, courseService, coinsService);

//        // Assert
//        Assert.Equal(200, viewModel.CoinBalance); // DummyCoinsService returns 200
//    }

//    [Fact]
//    public void ShortDescription_ShouldBeTruncated_WhenLong()
//    {
//        // Arrange
//        var module = GetTestModule();

//        // Act
//        var result = module.ShortDescription;

//        // Assert
//        Assert.True(result.EndsWith("..."));
//        Assert.Equal("This is a test descript...", result);
//    }

//    [Fact]
//    public void CanCompleteModule_ShouldReturnFalse_WhenAlreadyCompleted()
//    {
//        // Arrange
//        var module = GetTestModule();
//        var courseService = new DummyCourseService();
//        var coinsService = new DummyCoinsService();
//        var courseVM = new DummyCourseViewModel();

//        // Act
//        var viewModel = new ModuleViewModel(module, courseVM, courseService, coinsService);
//        var result = viewModel.CompleteModuleCommand.CanExecute(null);

//        // Assert
//        Assert.False(result); // Since we use the dummy course service, it's not marked as completed
//    }
//}

