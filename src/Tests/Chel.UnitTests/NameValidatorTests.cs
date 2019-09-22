using Xunit;

namespace Chel.UnitTests
{
    public class NameValidatorTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\r\n")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("-name")]
        public void IsValid_NameIsInvalid_ReturnsFalse(string name)
        {
            // arrange
            var sut = new NameValidator();

            // act
            var result = sut.IsValid(name);

            // assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("command")]
        [InlineData("COMMAND")]
        [InlineData("cØmÆnd")]
        [InlineData("±⇸")]
        [InlineData("👏")]
        public void IsValid_NameIsValid_ReturnsTrue(string name)
        {
            // arrange
            var sut = new NameValidator();

            // act
            var result = sut.IsValid(name);

            // assert
            Assert.True(result);
        }
    }
}