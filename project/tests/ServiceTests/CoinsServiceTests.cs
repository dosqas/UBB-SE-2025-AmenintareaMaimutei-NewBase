using System;
using Xunit;
using Moq;
using CourseApp.Repository;
using CourseApp.Services;

namespace CourseApp.Tests
{


    public class CoinsServiceTests
    {
        [Fact]
        public void GetCoinBalance_ReturnsCorrectBalance()
        {
            var fakeRepo = new FakeCoinsRepository();
            var coinsService = new CoinsService(fakeRepo);

            int balance = coinsService.GetCoinBalance(0);

            Assert.Equal(100, balance);
        }

        [Fact]
        public void TrySpendingCoins_DeductsCoinsSuccessfully()
        {
            var fakeRepo = new FakeCoinsRepository();
            var coinsService = new CoinsService(fakeRepo);

            bool result = coinsService.TrySpendingCoins(0, 50);

            Assert.True(result);
            Assert.Equal(50, coinsService.GetCoinBalance(0));
        }

        [Fact]
        public void ApplyDailyLoginBonus_AddsCoinsWhenLoginIsNewDay()
        {
            var fakeRepo = new FakeCoinsRepository();
            var coinsService = new CoinsService(fakeRepo);

            bool result = coinsService.ApplyDailyLoginBonus(0);

            Assert.True(result);
            Assert.Equal(200, coinsService.GetCoinBalance(0));
        }
    }

}
