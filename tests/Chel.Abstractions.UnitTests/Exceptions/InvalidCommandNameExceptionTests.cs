using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class InvalidCommandNameExceptionTests
    {
        [Fact]
        public void Ctor_WhenCommandNameIsNull_SetsCommandNamePropertyToNull()
        {
            // arrange, act
            var sut = new InvalidCommandNameException(null);

            // assert
            Assert.Null(sut.CommandName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("bad Command")]
        public void Ctor_WhenCalled_SetsCommandNameProperty(string commandName)
        {
            // arrange, act
            var sut = new InvalidCommandNameException(commandName);

            // assert
            Assert.Equal(commandName, sut.CommandName);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsMessage()
        {
            // arrange
            var sut = new InvalidCommandNameException("bad name");

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("The command name 'bad name' is invalid", result);
        }

        [Fact]
        public void ToString_NullCommandName_ReturnsMessage()
        {
            // arrange
            var sut = new InvalidCommandNameException(null);

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("The command name null is invalid", result);
        }
    }
}