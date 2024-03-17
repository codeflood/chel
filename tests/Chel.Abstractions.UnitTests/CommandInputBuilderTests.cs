using System;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandInputBuilderTests
    {
        private readonly static ExecutionTargetIdentifier SampleCommandIdentifier = new ExecutionTargetIdentifier(null, "command");

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            Action sutAction = () => new CommandInput.Builder(location, new ExecutionTargetIdentifier(null, ""));

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("Name", ex.ParamName);
            Assert.Contains("'Name' cannot be empty", ex.Message);
        }

        [Fact]
        public void AddParameter_ParameterIsNull_ThrowsException()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var sut = new CommandInput.Builder(location, SampleCommandIdentifier);
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
            var sut = new CommandInput.Builder(location, SampleCommandIdentifier);

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal(location, commandInput.SourceLocation);
            Assert.Equal("command", commandInput.CommandIdentifier.Name);
        }

        [Fact]
        public void Build_AfterAddParameterCalled_ReturnsCommandInputWithNumberedParameter()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var sut = new CommandInput.Builder(location, SampleCommandIdentifier);
            sut.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 1), new Literal("value1")));
            sut.AddParameter(new SourceValueCommandParameter(new SourceLocation(2, 1), new Literal("value2")));

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal("value1", ((commandInput.Parameters[0] as SourceValueCommandParameter).Value as Literal).Value);
            Assert.Equal("value2", ((commandInput.Parameters[1] as SourceValueCommandParameter).Value as Literal).Value);
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
            var sut = new CommandInput.Builder(location, SampleCommandIdentifier);
            sut.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 1), new Literal(value)));

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal(value, ((commandInput.Parameters[0] as SourceValueCommandParameter).Value as Literal).Value);
        }
    }
}