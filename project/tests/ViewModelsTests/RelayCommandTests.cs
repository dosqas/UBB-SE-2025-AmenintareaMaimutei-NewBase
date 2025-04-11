namespace Tests.ViewModelsTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;
    using CourseApp.ViewModels;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class RelayCommandTests
    {
        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenExecuteIsNull()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RelayCommand(null));
        }

        [Fact]
        public void CanExecute_ShouldReturnTrue_WhenNoPredicateProvided()
        {
            // Arrange
            var command = new RelayCommand(_ => { });

            // Act
            var result = command.CanExecute(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanExecute_ShouldReturnPredicateResult_WhenPredicateProvided()
        {
            // Arrange
            var command = new RelayCommand(_ => { }, _ => false);

            // Act
            var result = command.CanExecute(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Execute_ShouldInvokeExecuteAction()
        {
            // Arrange
            bool executed = false;
            var command = new RelayCommand(_ => executed = true);

            // Act
            command.Execute(null);

            // Assert
            Assert.True(executed);
        }

        [Fact]
        public void RaiseCanExecuteChanged_ShouldTriggerCanExecuteChangedEvent()
        {
            // Arrange
            var command = new RelayCommand(_ => { });
            bool eventRaised = false;

            command.CanExecuteChanged += (_, _) => eventRaised = true;

            // Act
            command.RaiseCanExecuteChanged();

            // Assert
            Assert.True(eventRaised);
        }

        [Fact]
        public void RaiseCanExecuteChanged_ShouldNotThrow_WhenNoSubscribers()
        {
            // Arrange
            var command = new RelayCommand(_ => { });

            // Act & Assert
            var exception = Record.Exception(() => command.RaiseCanExecuteChanged());

            Assert.Null(exception);
        }

    }
}
