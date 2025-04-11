﻿namespace Tests.ViewModelsTests
{
    using System.Collections.Generic;
    using CourseApp.Models;
    using CourseApp.Services;
    using CourseApp.ViewModels;
    using Moq;
    using Xunit;

    public class MainViewModelTests
    {
        private readonly Mock<ICourseService> mockCourseService;
        private readonly Mock<ICoinsService> mockCoinsService;
        private readonly IMainViewModel viewModel;

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
        public void Constructor_WhenInitialized_ShouldInitializeDisplayedCoursesAndAvailableTags()
        {
            // Arrange
            // Made in constructor

            // Act
            var viewModel = new MainViewModel(this.mockCourseService.Object, this.mockCoinsService.Object);

            // Assert
            Assert.NotNull(viewModel.DisplayedCourses);
            Assert.NotNull(viewModel.AvailableTags);
        }

        [Fact]
        public void Constructor_ShouldSubscribeToTagChanges_WhenTagsAreAvailable()
        {
            // Arrange
            var tag1 = new Tag { TagId = 1, Name = "AI", IsSelected = true };
            var tag2 = new Tag { TagId = 2, Name = "Web", IsSelected = true };
            var fakeTags = new List<Tag> { tag1, tag2 };

            this.mockCourseService.Setup(s => s.GetTags()).Returns(fakeTags);
            this.mockCourseService.Setup(s => s.GetCourses()).Returns(new List<Course>());

            // Act
            var viewModel = new MainViewModel(this.mockCourseService.Object, this.mockCoinsService.Object);

            // Assert
            Assert.NotNull(viewModel.AvailableTags);
            Assert.Equal(2, viewModel.AvailableTags.Count);
            Assert.True(viewModel.AvailableTags.All(t => t.IsSelected));
        }

        [Fact]
        public void SearchQuery_WhenSet_ShouldTriggerFiltering()
        {
            // Arrange
            var wasCalled = false;
            this.mockCourseService
                .Setup(s => s.GetFilteredCourses(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<List<int>>()))
                .Callback(() => wasCalled = true)
                .Returns(new List<Course>());

            // Act
            this.viewModel.SearchQuery = "Test";

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void FilterByPremium_WhenSet_ShouldTriggerFiltering()
        {
            // Arrange
            var wasCalled = false;
            this.mockCourseService
                .Setup(s => s.GetFilteredCourses(It.IsAny<string>(), true, false, false, false, It.IsAny<List<int>>()))
                .Callback(() => wasCalled = true)
                .Returns(new List<Course>());

            // Act
            this.viewModel.FilterByPremium = true;

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void FilterByFree_WhenSet_ShouldTriggerFiltering()
        {
            // Arrange
            var wasCalled = false;
            this.mockCourseService
                .Setup(s => s.GetFilteredCourses(It.IsAny<string>(), false, true, false, false, It.IsAny<List<int>>()))
                .Callback(() => wasCalled = true)
                .Returns(new List<Course>());

            // Act
            this.viewModel.FilterByFree = true;

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void FilterByEnrolled_WhenSet_ShouldTriggerFiltering()
        {
            // Arrange
            var wasCalled = false;
            this.mockCourseService
                .Setup(s => s.GetFilteredCourses(It.IsAny<string>(), false, false, true, false, It.IsAny<List<int>>()))
                .Callback(() => wasCalled = true)
                .Returns(new List<Course>());

            // Act
            this.viewModel.FilterByEnrolled = true;

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void FilterByNotEnrolled_WhenSet_ShouldTriggerFiltering()
        {
            // Arrange
            var wasCalled = false;
            this.mockCourseService
                .Setup(s => s.GetFilteredCourses(It.IsAny<string>(), false, false, false, true, It.IsAny<List<int>>()))
                .Callback(() => wasCalled = true)
                .Returns(new List<Course>());

            // Act
            this.viewModel.FilterByNotEnrolled = true;

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void TryDailyLoginReward_WhenSuccessful_ShouldUpdateUserCoinBalance()
        {
            // Arrange
            this.mockCoinsService.Setup(s => s.ApplyDailyLoginBonus(It.IsAny<int>())).Returns(true);
            var notifiedProps = new List<string>();
            this.viewModel.PropertyChanged += (s, e) => notifiedProps.Add(e.PropertyName);

            // Act
            var result = this.viewModel.TryDailyLoginReward();

            // Assert
            Assert.True(result);
            Assert.Contains(nameof(this.viewModel.UserCoinBalance), notifiedProps);
        }

        [Fact]
        public void ResetAllFiltersCommand_WhenExecuted_ShouldClearAllFilters()
        {
            // Arrange
            var tag1 = new Tag { TagId = 1, Name = "AI", IsSelected = true };
            var tag2 = new Tag { TagId = 2, Name = "Web", IsSelected = true };
            var fakeTags = new List<Tag> { tag1, tag2 };

            var fakeCourses = new List<Course>
            {
                new Course
                {
                    CourseId = 1,
                    Title = "Test Java Course",
                    Description = "Description",
                    ImageUrl = "url",
                    Difficulty = "Easy",
                    IsPremium = true,
                },
            };

            this.mockCourseService.Setup(s => s.GetCourses()).Returns(fakeCourses);
            this.mockCourseService.Setup(s => s.GetTags()).Returns(fakeTags);
            this.mockCourseService.Setup(s => s.GetFilteredCourses(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<List<int>>()))
                .Returns(fakeCourses);

            // Act
            this.viewModel.SearchQuery = "Java";
            this.viewModel.FilterByPremium = true;
            this.viewModel.FilterByFree = true;
            this.viewModel.FilterByEnrolled = true;
            this.viewModel.FilterByNotEnrolled = true;

            foreach (var tag in this.viewModel.AvailableTags)
            {
                tag.IsSelected = true;
            }

            this.viewModel.ResetAllFiltersCommand.Execute(null);

            // Assert
            Assert.Equal(string.Empty, this.viewModel.SearchQuery);
            Assert.False(this.viewModel.FilterByPremium);
            Assert.False(this.viewModel.FilterByFree);
            Assert.False(this.viewModel.FilterByEnrolled);
            Assert.False(this.viewModel.FilterByNotEnrolled);
            Assert.All(this.viewModel.AvailableTags, tag => Assert.False(tag.IsSelected));
        }

        [Fact]
        public void SearchQuery_SetToSameValue_ShouldNotTriggerFilter()
        {
            // Arrange
            var wasCalled = false;

            this.mockCourseService
                .Setup(s => s.GetFilteredCourses("same", false, false, false, false, It.IsAny<List<int>>()))
                .Callback(() => wasCalled = true)
                .Returns(new List<Course>());

            this.viewModel.SearchQuery = "same";

            wasCalled = false;

            // Act
            this.viewModel.SearchQuery = "same";

            // Assert
            Assert.False(wasCalled);
        }

        [Fact]
        public void SearchQuery_SetOverMaxLength_ShouldNotChangeProperty()
        {
            // Arrange
            var initialSearchQuery = this.viewModel.SearchQuery;
            string longSearchQuery = new string('a', 101); // Assuming the max length is 100.

            // Act
            this.viewModel.SearchQuery = longSearchQuery;

            // Assert
            Assert.Equal(initialSearchQuery, this.viewModel.SearchQuery);
        }

        [Fact]
        public void UserCoinBalance_ShouldReturnCorrectBalance_FromCoinsService()
        {
            // Arrange
            var expectedBalance = 42;
            var coinsServiceMock = new Mock<ICoinsService>();
            coinsServiceMock
                .Setup(cs => cs.GetCoinBalance(It.IsAny<int>()))
                .Returns(expectedBalance);

            var courseServiceMock = new Mock<ICourseService>();
            courseServiceMock.Setup(cs => cs.GetCourses()).Returns(new List<Course>());
            courseServiceMock.Setup(cs => cs.GetTags()).Returns(new List<Tag>());

            var viewModel = new MainViewModel(courseServiceMock.Object, coinsServiceMock.Object);

            // Act
            var actualBalance = viewModel.UserCoinBalance;

            // Assert
            Assert.Equal(expectedBalance, actualBalance);
        }
    }
}
