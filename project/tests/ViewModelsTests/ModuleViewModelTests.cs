namespace Tests.ModuleViewTests
{

    using Xunit;
    using Moq;
    using CourseApp.Models;
    using CourseApp.Services;
    using CourseApp.ViewModels;
    using CourseApp.Repository;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ModuleViewModelTests
    {
        [Fact]
        public void Constructor_OpensModuleAndSetsIsCompleted()
        {
            // Arrange
            var module = new Module
            {
                ModuleId = 1,
                Title = "Test Module",
                Description = "Test Description",
                ImageUrl = "test_image.jpg"
            };
            var mockCourseService = new Mock<ICourseService>();
            var mockCoinsService = new Mock<ICoinsService>();
            var mockCourseVM = new Mock<CourseViewModel>();

            mockCourseService.Setup(cs => cs.IsModuleCompleted(1)).Returns(true);

            // Act
            var viewModel = new ModuleViewModel(module, mockCourseVM.Object,
                                                mockCourseService.Object,
                                                mockCoinsService.Object);

            // Assert
            Assert.True(viewModel.IsCompleted);
            mockCourseService.Verify(cs => cs.OpenModule(1), Times.Exactly(2)); // called twice in ctor
        }

        [Fact]
        public void HandleModuleImageClick_AddsCoinsAndRaisesCoinBalanceChanged()
        {
            // Arrange
            var module = new Module
            {
                ModuleId = 1,
                Title = "Test Module",
                Description = "Test Description",
                ImageUrl = "test_image.jpg"
            };
            var mockCourseService = new Mock<ICourseService>();
            var mockCoinsService = new Mock<ICoinsService>();
            var mockCourseVM = new Mock<CourseViewModel>();

            mockCourseService.Setup(cs => cs.IsModuleCompleted(It.IsAny<int>())).Returns(false);
            mockCourseService.Setup(cs => cs.ClickModuleImage(1)).Returns(true);
            var viewModel = new ModuleViewModel(module, mockCourseVM.Object,
                                                mockCourseService.Object,
                                                mockCoinsService.Object);

            bool coinChanged = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.CoinBalance))
                    coinChanged = true;
            };

            // Act
            viewModel.HandleModuleImageClick(null);

            // Assert
            Assert.True(coinChanged);
        }

        [Fact]
        public void ExecuteCompleteModule_SetsIsCompleted_AndNotifies()
        {
            // Arrange
            var module = new Module
            {
                ModuleId = 1,
                Title = "Test Module",
                Description = "Test Description",
                ImageUrl = "test_image.jpg"
            };

            var mockCourseService = new Mock<ICourseService>();
            var mockCoinsService = new Mock<ICoinsService>();
            var mockCourseVM = new Mock<ICourseViewModel>();

            mockCourseService.Setup(cs => cs.IsModuleCompleted(It.IsAny<int>())).Returns(false);

            var viewModel = new ModuleViewModel(module, mockCourseVM.Object,
                                                mockCourseService.Object,
                                                mockCoinsService.Object);

            bool isCompletedChanged = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.IsCompleted))
                    isCompletedChanged = true;
            };

            // Act
            viewModel.CompleteModuleCommand.Execute(null);

            // Assert
            Assert.True(viewModel.IsCompleted);
            Assert.True(isCompletedChanged);
            mockCourseVM.Verify(vm => vm.MarkModuleAsCompletedAndCheckRewards(1), Times.Once);
            mockCourseVM.Verify(vm => vm.RefreshCourseModulesDisplay(), Times.Once);
        }

        [Fact]
        public void CoinBalance_GetsValueFromCoinsService()
        {
            // Arrange
            var module = new Module
            {
                ModuleId = 1,
                Title = "Test Module",
                Description = "Test Description",
                ImageUrl = "test_image.jpg"
            };

            var mockCourseService = new Mock<ICourseService>();
            var mockCoinsService = new Mock<ICoinsService>();
            var mockCourseVM = new Mock<CourseViewModel>();

            // Setup the mock correctly - use the interface method that will actually be called
            mockCoinsService.Setup(cs => cs.GetCoinBalance(0)).Returns(123);
            var mockService = mockCoinsService.Object;
            var viewModel = new ModuleViewModel(
                module,
                mockCourseVM.Object,
                mockCourseService.Object,  // Use the interface, don't cast
                mockCoinsService.Object   // Use the interface, don't cast
            );

            // Act
            var balance = viewModel.CoinBalance;
            Console.WriteLine($"Actual balance: {balance}");

            // Assert
            Assert.Equal(123, balance);
            mockCoinsService.Verify(cs => cs.GetCoinBalance(0), Times.Once);
        }

        [Fact]
        public void TimeSpent_ReturnsFormattedTimeFromCourseViewModel()
        {
            // Arrange
            var module = new Module
            {
                ModuleId = 1,
                Title = "Test Module",
                Description = "Test Description",
                ImageUrl = "test_image.jpg"
            };
            var mockCourseService = new Mock<ICourseService>();
            var mockCoinsService = new Mock<ICoinsService>();
            var mockCourseVM = new Mock<ICourseViewModel>();
            mockCourseVM.Setup(vm => vm.FormattedTimeRemaining).Returns("58 min 23s");

            var viewModel = new ModuleViewModel(module, mockCourseVM.Object,
                mockCourseService.Object, mockCoinsService.Object);

            // Act
            var timeSpent = viewModel.TimeSpent;

            // Assert
            Assert.Equal("58 min 23s", timeSpent);
        }
        [Fact]
        public void ExecuteModuleImageClick_TriggersCoinBalanceAndRefresh()
        {
            // Arrange
            var module = new Module
            {
                ModuleId = 1,
                Title = "Test Module",
                Description = "Test Description",
                ImageUrl = "test_image.jpg"
            };
            var mockCourseService = new Mock<ICourseService>();
            var mockCoinsService = new Mock<ICoinsService>();
            var mockCourseVM = new Mock<ICourseViewModel>();

            mockCourseService.Setup(cs => cs.IsModuleCompleted(It.IsAny<int>())).Returns(false);
            mockCourseService.Setup(cs => cs.ClickModuleImage(module.ModuleId)).Returns(true);

            var viewModel = new ModuleViewModel(module, mockCourseVM.Object,
                mockCourseService.Object, mockCoinsService.Object);

            bool coinChanged = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.CoinBalance))
                    coinChanged = true;
            };

            // Act
            viewModel.ExecuteModuleImageClick(null);

            // Assert
            Assert.True(coinChanged);
            mockCourseVM.Verify(vm => vm.RefreshCourseModulesDisplay(), Times.Once);
        }
        [Fact]
        public void CanCompleteModule_ReturnsFalse_WhenIsCompletedIsTrue()
        {
            // Arrange
            var module = new Module
            {
                ModuleId = 1,
                Title = "Test Module",
                Description = "Test Description",
                ImageUrl = "test_image.jpg"
            };
            var mockCourseService = new Mock<ICourseService>();
            mockCourseService.Setup(cs => cs.IsModuleCompleted(1)).Returns(true);

            var viewModel = new ModuleViewModel(module, new Mock<CourseViewModel>().Object,
                mockCourseService.Object, new Mock<ICoinsService>().Object);

            // Act
            var canComplete = viewModel.CompleteModuleCommand.CanExecute(null);

            // Assert
            Assert.False(canComplete);
        }

        [Fact]
        public void ChangingFormattedTimeRemaining_RaisesTimeSpentPropertyChanged()
        {
            // Arrange
            var module = new Module
            {
                ModuleId = 1,
                Title = "Test Module",
                Description = "Test Description",
                ImageUrl = "test_image.jpg"
            };
            var mockCourseService = new Mock<ICourseService>();
            mockCourseService.Setup(cs => cs.IsModuleCompleted(It.IsAny<int>())).Returns(false);

            var mockCoinsService = new Mock<ICoinsService>();

            var mockCourseVM = new Mock<ICourseViewModel>();
            mockCourseVM.Setup(vm => vm.FormattedTimeRemaining).Returns("10 min");

            var viewModel = new ModuleViewModel(module, mockCourseVM.Object,
                                                mockCourseService.Object, mockCoinsService.Object);

            bool timeSpentChanged = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.TimeSpent))
                    timeSpentChanged = true;
            };

            // Act: raise PropertyChanged for FormattedTimeRemaining on the mock
            mockCourseVM.Raise(vm => vm.PropertyChanged += null, new PropertyChangedEventArgs(nameof(mockCourseVM.Object.FormattedTimeRemaining)));

            // Assert
            Assert.True(timeSpentChanged);
        }
        [Fact]
        public void ShortDescription_ShouldReturnShortenedDescription_WhenDescriptionIsLongerThan23Characters()
        {
            // Arrange
            var module = new Module
            {
                ModuleId = 1,
                Title = "Test Module",
                Description = "This is a longer description for the module.",
                ImageUrl = "test_image.jpg"
            };

            // Act
            var result = module.ShortDescription;

            // Assert
            Assert.Equal("This is a longer descri...", result);
        }
    }
}