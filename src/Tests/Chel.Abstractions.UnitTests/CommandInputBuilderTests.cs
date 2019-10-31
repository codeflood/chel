using System;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandInputBuilderTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Ctor_SourceLineIsNonPositive_ThrowsException(int sourceLine)
        {
            // arrange
            Action sutAction = () => new CommandInput.Builder(sourceLine, "command");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("sourceLine", ex.ParamName);
            Assert.Contains("sourceLine must be greater than 0", ex.Message);
        }

        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandInput.Builder(1, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandInput.Builder(1, "");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
            Assert.Contains("commandName cannot be empty", ex.Message);
        }

        [Fact]
        public void AddNumberedParameter_ParameterIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandInput.Builder(1, "command");
            Action sutAction = () => sut.AddNumberedParameter(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void Build_WhenCalled_ReturnsCommandInput()
        {
            // arrange
            var sut = new CommandInput.Builder(3, "command");

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal(3, commandInput.SourceLine);
            Assert.Equal("command", commandInput.CommandName);
        }

        [Fact]
        public void Build_AfterAddNumberedParameterCalled_ReturnsCommandInputWithNumberedParameter()
        {
            // arrange
            var sut = new CommandInput.Builder(3, "command");
            sut.AddNumberedParameter("value1");
            sut.AddNumberedParameter("value2");

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal("value1", commandInput.NumberedParameters[0]);
            Assert.Equal("value2", commandInput.NumberedParameters[1]);
        }
    }
}