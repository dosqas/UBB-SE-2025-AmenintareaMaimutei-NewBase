namespace Tests.ConvertersTests
{
    using System;
    using CourseApp.Converters;
    using Microsoft.UI.Xaml.Media;
    using Windows.UI;
    using Xunit;

    public class AvailabilityColorConverterTests
    {
        private readonly AvailabilityColorConverter testConverter;

        public AvailabilityColorConverterTests()
        {
            this.testConverter = new AvailabilityColorConverter();
        }

        /// <summary>
        /// Tests that the ConvertBack method throws a NotImplementedException.
        /// </summary>
        [Fact]
        public void ConvertBackShouldThrowNotImplementedException()
        {
            // Arrange
            var input = true;

            // Act & Assert
            var exception = Assert.Throws<NotImplementedException>(() =>
                this.testConverter.ConvertBack(input, typeof(bool), null!, null!));
            Assert.Equal("Reverse conversion is not supported", exception.Message);
        }
    }
}
