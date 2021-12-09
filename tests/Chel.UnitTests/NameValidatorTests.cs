using Xunit;

namespace Chel.UnitTests
{
    public class NameValidatorTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("na me")]
        [InlineData("\r\n")]
        [InlineData("na\r\nme")]
        [InlineData("\n")]
        [InlineData("na\nme")]
        [InlineData("\t")]
        [InlineData("na\tme")]
        [InlineData("-name")]
        [InlineData("(")]
        [InlineData(")")]
        [InlineData("(name)")]
        [InlineData("[")]
        [InlineData("]")]
        [InlineData("na[me]")]
        [InlineData("{")]
        [InlineData("}")]
        [InlineData("\\")]
        [InlineData("na\\me")]
        [InlineData("#")]
        [InlineData("na#me")]
        [InlineData("$")]
        [InlineData("na$me")]
        [InlineData(":")]
        [InlineData("na:me")]
        [InlineData("*name")]
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
        [InlineData("a")]
        [InlineData("command")]
        [InlineData("comm-and")]
        [InlineData("COMMAND")]
        [InlineData("cØmÆnd")]
        [InlineData("±⇸")]
        [InlineData("👏")]
        [InlineData("comm*and")]
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