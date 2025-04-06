namespace tests.ConvertersTests
{
    using CourseApp.Converters;
    using Microsoft.UI;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Media;
    using Windows.UI;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="InverseBooleanConverter"/> class.
    /// </summary>
    public class InverseBooleanConverterTests
    {
        private readonly InverseBooleanConverter TestConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="InverseBooleanConverterTests"/> class.
        /// </summary>
        public InverseBooleanConverterTests()
        {
            // Initialize the converter before each test
            this.TestConverter = new InverseBooleanConverter();
        }

        [Fact]
        public void ConvertShouldReturnFalseWhenTrue()
        {
            // Arrange
            var input = true;

            // Act
            var result = this.TestConverter.Convert(input, typeof(bool), null, string.Empty);

            // Assert
            Assert.False((bool)result);
        }

        [Fact]
        public void ConvertShouldReturnTrueWhenFalse()
        {
            // Arrange
            var input = false;

            // Act
            var result = this.TestConverter.Convert(input, typeof(bool), null, string.Empty);

            // Assert
            Assert.True((bool)result);
        }

        [Fact]
        public void ConvertShouldReturnOriginalValueWhenInputIsNotBoolean()
        {
            // Arrange
            var input = "string";

            // Act
            var result = this.TestConverter.Convert(input, typeof(bool), null, string.Empty);

            // Assert
            Assert.Equal(input, result);
        }

        [Fact]
        public void ConvertBackShouldReturnFalseWhenTrue()
        {
            // Arrange
            var input = true;

            // Act
            var result = this.TestConverter.ConvertBack(input, typeof(bool), null, string.Empty);

            // Assert
            Assert.False((bool)result);
        }

        [Fact]
        public void ConvertBackShouldReturnTrueWhenFalse()
        {
            // Arrange
            var input = false;

            // Act
            var result = this.TestConverter.ConvertBack(input, typeof(bool), null, string.Empty);

            // Assert
            Assert.True((bool)result);
        }

        [Fact]
        public void ConvertBackShouldReturnOriginalValueWhenInputIsNotBoolean()
        {
            // Arrange
            var input = "string";

            // Act
            var result = this.TestConverter.ConvertBack(input, typeof(bool), null, string.Empty);

            // Assert
            Assert.Equal(input, result);
        }
    }
}
