//using Xunit;
//using CourseApp.ViewModels;
//using CourseApp.Models;
//using CourseApp.Services;
//using Moq;
//using System.ComponentModel;

//namespace Tests.moduleViewModelTests
//{
//    public class ModuleViewModelTests
//    {
//        private readonly Mock<ICourseService> mockCourseService = new();
//        private readonly Mock<ICoinsService> mockCoinsService = new();
//        private readonly Mock<ICourseViewModel> mockCourseVM = new();

//        private readonly Module testModule = new()
//        {
//            ModuleId = 1,
//            Title = "Test Module",
//            Description = "Description",
//            ImageUrl = "http://example.com/image.png",
//        };

//        public ModuleViewModelTests()
//        {
//            mockCourseVM.Setup(vm => vm.FormattedTimeRemaining).Returns("00:10:00");
//            mockCourseVM.Raise(vm => vm.PropertyChanged += null,
//                new PropertyChangedEventArgs(nameof(ICourseViewModel.FormattedTimeRemaining)));
//        }

//        [Fact]
//        public void Constructor_InitializesPropertiesCorrectly()
//        {
//            mockCourseService.Setup(s => s.IsModuleCompleted(1)).Returns(false);

//            var vm = new ModuleViewModel(testModule, mockCourseVM.Object,
//                                         mockCourseService.Object, mockCoinsService.Object);

//            Assert.Equal(testModule, vm.CurrentModule);
//            Assert.True(vm.IsCompleted);
//            Assert.Equal("00:10:00", vm.TimeSpent);
//        }

//        [Fact]
//        public void ExecuteCompleteModule_SetsIsCompletedToTrue()
//        {
//            mockCourseService.Setup(s => s.IsModuleCompleted(1)).Returns(false);

//            var vm = new ModuleViewModel(testModule, mockCourseVM.Object,
//                                         mockCourseService.Object, mockCoinsService.Object);

//            mockCourseVM.Setup(vm => vm.MarkModuleAsCompletedAndCheckRewards(1));

//            vm.CompleteModuleCommand.Execute(null);

//            Assert.True(vm.IsCompleted);
//        }

//        [Fact]
//        public void HandleModuleImageClick_UpdatesCoinBalance()
//        {
//            mockCourseService.Setup(s => s.ClickModuleImage(1)).Returns(true);
//            mockCoinsService.Setup(c => c.GetCoinBalance(It.IsAny<int>())).Returns(100);

//            var vm = new ModuleViewModel(testModule, mockCourseVM.Object,
//                                         mockCourseService.Object, mockCoinsService.Object);

//            bool coinBalanceChanged = false;
//            vm.PropertyChanged += (s, e) =>
//            {
//                if (e.PropertyName == nameof(vm.CoinBalance))
//                    coinBalanceChanged = true;
//            };

//            vm.HandleModuleImageClick(null);

//            Assert.False(coinBalanceChanged);
//        }

//        [Fact]
//        public void CoinBalance_ReturnsCorrectValue()
//        {
//            mockCoinsService.Setup(c => c.GetCoinBalance(0)).Returns(42);

//            var vm = new ModuleViewModel(testModule, mockCourseVM.Object,
//                                         mockCourseService.Object, mockCoinsService.Object);

//            Assert.Equal(562, vm.CoinBalance);
//        }
//    }
//}
