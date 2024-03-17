using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class InvalidNameExceptionTests
    {
        [Fact]
        public void Ctor_WhenNameIsNull_SetsNamePropertyToNull()
        {
            // arrange, act
            var sut = new InvalidNameException(null);

            // assert
            Assert.Null(sut.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("bad Command")]
        public void Ctor_WhenCalled_SetsNameProperty(string name)
        {
            // arrange, act
            var sut = new InvalidNameException(name);

            // assert
            Assert.Equal(name, sut.Name);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsMessage()
        {
            // arrange
            var sut = new InvalidNameException("bad name");

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("The name 'bad name' is invalid", result);
        }

        [Fact]
        public void ToString_NullName_ReturnsMessage()
        {
            // arrange
            var sut = new InvalidNameException(null);

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("The name cannot be null", result);
        }
    }
}