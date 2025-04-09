namespace Tests.ViewModelsTests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using CourseApp.Models;
    using CourseApp.Services;
    using CourseApp.ViewModels;
    using Moq;
    using Xunit;

    public class MainViewModelTests
    {
        private readonly Mock<ICourseService> mockCourseService;
        private readonly Mock<ICoinsService> mockCoinsService;
        private readonly MainViewModel viewModel;

        public MainViewModelTests()
        {
            this.mockCourseService = new Mock<ICourseService>();
            this.mockCoinsService = new Mock<ICoinsService>();

            var fakeCourses = new List<Course> { new Course { CourseId = 1, Title = "Test", Description = "Description", ImageUrl = "url", Difficulty = "Easy" } };
            var fakeTags = new List<Tag> { new Tag { TagId = 1, Name = "AI" } };

            this.mockCourseService.Setup(s => s.GetCourses()).Returns(fakeCourses);
            this.mockCourseService.Setup(s => s.GetTags()).Returns(fakeTags);
            this.mockCoinsService.Setup(s => s.GetCoinBalance(It.IsAny<int>())).Returns(100);

            this.viewModel = new MainViewModel(this.mockCourseService.Object, this.mockCoinsService.Object);
        }

        [Fact]
        public void ConstructorInitializesDisplayedCoursesAndAvailableTags()
        {
            Assert.NotNull(this.viewModel.DisplayedCourses);
            Assert.NotNull(this.viewModel.AvailableTags);
        }

        [Fact]
        public void SearchQuery_SetValue_TriggersFiltering()
        {
            var wasCalled = false;
            this.mockCourseService.Setup(s => s.GetFilteredCourses(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<List<int>>()))
                              .Callback(() => wasCalled = true)
                              .Returns(new List<Course>());

            this.viewModel.SearchQuery = "Test";

            Assert.True(wasCalled);
        }

        [Fact]
        public void FilterByPremium_SetValue_TriggersFiltering()
        {
            var wasCalled = false;
            this.mockCourseService.Setup(s => s.GetFilteredCourses(It.IsAny<string>(), true, false, false, false, It.IsAny<List<int>>()))
                              .Callback(() => wasCalled = true)
                              .Returns(new List<Course>());

            this.viewModel.FilterByPremium = true;

            Assert.True(wasCalled);
        }

        [Fact]
        public void TryDailyLoginReward_WhenSuccessful_UpdatesUserCoinBalance()
        {
            this.mockCoinsService.Setup(s => s.ApplyDailyLoginBonus(It.IsAny<int>())).Returns(true);
            var notifiedProps = new List<string>();
            this.viewModel.PropertyChanged += (s, e) => notifiedProps.Add(e.PropertyName);

            var result = this.viewModel.TryDailyLoginReward();

            Assert.True(result);
            Assert.Contains(nameof(this.viewModel.UserCoinBalance), notifiedProps);
        }

        //[Fact]
        //public void ResetAllFiltersClearsAllFilters()
        //{
        //    // Define tags
        //    var tag1 = new Tag { TagId = 1, Name = "AI", IsSelected = true };
        //    var tag2 = new Tag { TagId = 2, Name = "Web", IsSelected = true };
        //    var fakeTags = new List<Tag> { tag1, tag2 };

        //    // Define courses that use those tags
        //    var fakeCourses = new List<Course>
        //        {
        //            new Course
        //            {
        //                CourseId = 1,
        //                Title = "Test Java Course",
        //                Description = "Description",
        //                ImageUrl = "url",
        //                Difficulty = "Easy",
        //                IsPremium = true,
        //            },
        //        };

        //    this.mockCourseService.Setup(s => s.GetCourses()).Returns(fakeCourses);
        //    this.mockCourseService.Setup(s => s.GetTags()).Returns(fakeTags);

        //    var newViewModel = new MainViewModel(this.mockCourseService.Object, this.mockCoinsService.Object);

        //    // Set filters before resetting
        //    newViewModel.SearchQuery = "Java";
        //    newViewModel.FilterByPremium = true;
        //    newViewModel.FilterByFree = true;
        //    newViewModel.FilterByEnrolled = true;
        //    newViewModel.FilterByNotEnrolled = true;

        //    // Execute reset
        //    newViewModel.ResetAllFiltersCommand.Execute(null);

        //    // Assert all filters are cleared
        //    Assert.Equal(string.Empty, newViewModel.SearchQuery);
        //    Assert.False(newViewModel.FilterByPremium);
        //    Assert.False(newViewModel.FilterByFree);
        //    Assert.False(newViewModel.FilterByEnrolled);
        //    Assert.False(newViewModel.FilterByNotEnrolled);
        //    Assert.All(newViewModel.AvailableTags, tag => Assert.False(tag.IsSelected));
        //}
    }
}