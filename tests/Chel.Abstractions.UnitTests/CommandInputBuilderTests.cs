using System;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandInputBuilderTests
    {
        [Fact]
        public void Ctor_SourceLocationIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandInput.Builder(null, "command");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("sourceLocation", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            Action sutAction = () => new CommandInput.Builder(location, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            Action sutAction = () => new CommandInput.Builder(location, "");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
            Assert.Contains("'commandName' cannot be empty", ex.Message);
        }

        [Fact]
        public void AddParameter_ParameterIsNull_ThrowsException()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var sut = new CommandInput.Builder(location, "command");
            Action sutAction = () => sut.AddParameter(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void Build_WhenCalled_ReturnsCommandInput()
        {
            // arrange
            var location = new SourceLocation(2, 6);
            var sut = new CommandInput.Builder(location, "command");

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal(location, commandInput.SourceLocation);
            Assert.Equal("command", commandInput.CommandName);
        }

        [Fact]
        public void Build_AfterAddParameterCalled_ReturnsCommandInputWithNumberedParameter()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var sut = new CommandInput.Builder(location, "command");
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
            var location = new SourceLocation(1, 1);
            var sut = new CommandInput.Builder(location, "command");
            sut.AddParameter(new Literal(value));

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal(value, (commandInput.Parameters[0] as Literal).Value);
        }
    }
}