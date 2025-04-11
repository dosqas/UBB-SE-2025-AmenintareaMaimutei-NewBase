// <copyright file="CoinsServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CourseApp.Tests
{
    using System;
    using CourseApp.Repository;
    using CourseApp.Services;
    using Moq;
    using Xunit;

    /// <summary>
    /// Contains unit tests for the <see cref="CoinsService"/> class.
    /// </summary>
    public class CoinsServiceTests
    {
        /// <summary>
        /// Tests that <see cref="CoinsService.GetCoinBalance"/> returns the correct balance.
        /// </summary>
        [Fact]
        public void GetCoinBalance_ReturnsCorrectBalance()
        {
            var fakeRepo = new FakeCoinsRepository();
            var coinsService = new CoinsService(fakeRepo);

            int balance = coinsService.GetCoinBalance(0);

            Assert.Equal(100, balance);
        }

        /// <summary>
        /// Tests that <see cref="CoinsService.TrySpendingCoins"/> deducts coins successfully.
        /// </summary>
        [Fact]
        public void TrySpendingCoins_DeductsCoinsSuccessfully()
        {
            var fakeRepo = new FakeCoinsRepository();
            var coinsService = new CoinsService(fakeRepo);

            bool result = coinsService.TrySpendingCoins(0, 50);

            Assert.True(result);
            Assert.Equal(50, coinsService.GetCoinBalance(0));
        }

        /// <summary>
        /// Tests that <see cref="CoinsService.ApplyDailyLoginBonus"/> adds coins when the login is on a new day.
        /// </summary>
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
