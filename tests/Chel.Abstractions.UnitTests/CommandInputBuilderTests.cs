using System;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
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
            Assert.Contains("'sourceLine' must be greater than 0", ex.Message);
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
            Assert.Contains("'commandName' cannot be empty", ex.Message);
        }

        [Fact]
        public void AddParameter_ParameterIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandInput.Builder(1, "command");
            Action sutAction = () => sut.AddParameter(null);

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
        public void Build_AfterAddParameterCalled_ReturnsCommandInputWithNumberedParameter()
        {
            // arrange
            var sut = new CommandInput.Builder(3, "command");
            sut.AddParameter(new Literal("value1"));
            sut.AddParameter(new Literal("value2"));

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal("value1", (commandInput.Parameters[0] as Literal).Value);
            Assert.Equal("value2", (commandInput.Parameters[1] as Literal).Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData(" ")]
        public void Build_EmptyOrWhiteSpaceParameter_ReturnsCommandInputWithParameter(string value)
        {
            // arrange
            var sut = new CommandInput.Builder(3, "command");
            sut.AddParameter(new Literal(value));

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal(value, (commandInput.Parameters[0] as Literal).Value);
        }
    }
}