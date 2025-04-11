// <copyright file="CoinsServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests.ServiceTests
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
        private readonly FakeCoinsRepository _fakeRepo;
        private readonly CoinsService _coinsService;

        public CoinsServiceTests()
        {
            _fakeRepo = new FakeCoinsRepository();
            _coinsService = new CoinsService(_fakeRepo);
        }

        /// <summary>
        /// Tests that <see cref="CoinsService.GetCoinBalance"/> returns the correct balance.
        /// </summary>
        [Fact]
        public void GetCoinBalance_ReturnsCorrectBalance()
        {
            int balance = _coinsService.GetCoinBalance(0);
            Assert.Equal(100, balance);
        }

        /// <summary>
        /// Tests that <see cref="CoinsService.TrySpendingCoins"/> deducts coins successfully.
        /// </summary>
        [Fact]
        public void TrySpendingCoins_DeductsCoinsSuccessfully()
        {
            bool result = _coinsService.TrySpendingCoins(0, 50);
            Assert.True(result);
            Assert.Equal(50, _coinsService.GetCoinBalance(0));
        }

        [Fact]
        public void TrySpendingCoins_FailsWhenInsufficientFunds()
        {
            bool result = _coinsService.TrySpendingCoins(0, 150);
            Assert.False(result);
            Assert.Equal(100, _coinsService.GetCoinBalance(0));
        }

        [Fact]
        public void AddCoins_IncreasesBalanceCorrectly()
        {
            _coinsService.AddCoins(0, 50);
            Assert.Equal(150, _coinsService.GetCoinBalance(0));
        }

        /// <summary>
        /// Tests that <see cref="CoinsService.ApplyDailyLoginBonus"/> adds coins when the login is on a new day.
        /// </summary>
        [Fact]
        public void ApplyDailyLoginBonus_AddsCoinsWhenLoginIsNewDay()
        {
            bool result = _coinsService.ApplyDailyLoginBonus(0);
            Assert.True(result);
            Assert.Equal(200, _coinsService.GetCoinBalance(0));
        }

        [Fact]
        public void ApplyDailyLoginBonus_DoesNotAddCoinsWhenSameDay()
        {
            _coinsService.ApplyDailyLoginBonus(0);

            bool result = _coinsService.ApplyDailyLoginBonus(0);

            Assert.False(result);
            Assert.Equal(200, _coinsService.GetCoinBalance(0));
        }

        [Fact]
        public void Constructor_UsesDefaultRepositoryWhenNoneProvided()
        {
            var service = new CoinsService();
            Assert.NotNull(service); 
        }

        [Fact]
        public void GetCoinBalance_ReturnsZeroForNewUser()
        {
            _fakeRepo.SetUserCoinBalance(1, 0); 
            int balance = _coinsService.GetCoinBalance(1);
            Assert.Equal(0, balance);
        }
    }
}
