using System;
using Chel.Abstractions;
using Chel.UnitTests.SampleCommands;
using Xunit;

namespace Chel.UnitTests
{
    public class CommandParameterBinderTests
    {
        [Fact]
        public void Bind_InstanceIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var input = new CommandInput.Builder(1, "command").Build();
            Action sutAction = () => sut.Bind(null, input);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("instance", ex.ParamName);
        }

        [Fact]
        public void Bind_InputIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new SampleCommand();
            Action sutAction = () => sut.Bind(command, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("input", ex.ParamName);
        }

        [Fact]
        public void Bind_NumberedParameter1StringProperty_PropertyIsSet()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new NumberedParameterCommand();
            var builder = new CommandInput.Builder(1, "num");
            builder.AddNumberedParameter("value");
            var input = builder.Build();

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("value", command.NumberedParameter1);
        }

        [Fact]
        public void Bind_NumberedParameter2StringProperty_PropertyIsSet()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new NumberedParameterCommand();
            var builder = new CommandInput.Builder(1, "num");
            builder.AddNumberedParameter("value1");
            builder.AddNumberedParameter("value2");
            var input = builder.Build();

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("value1", command.NumberedParameter1);
            Assert.Equal("value2", command.NumberedParameter2);
        }

        [Fact]
        public void Bind_OneTooManyNumberedParametersProvided_ErrorIncludedInResult()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new NumberedParameterCommand();
            var builder = new CommandInput.Builder(1, "num");
            builder.AddNumberedParameter("value1");
            builder.AddNumberedParameter("value2");
            builder.AddNumberedParameter("value3");
            var input = builder.Build();

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unexpected numbered parameter value3" }, result.Errors);
        }

        [Fact]
        public void Bind_TwoTooManyNumberedParametersProvided_ErrorsIncludedInResult()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new NumberedParameterCommand();
            var builder = new CommandInput.Builder(1, "num");
            builder.AddNumberedParameter("value1");
            builder.AddNumberedParameter("value2");
            builder.AddNumberedParameter("value3");
            builder.AddNumberedParameter("value4");
            var input = builder.Build();

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unexpected numbered parameter value3", "Unexpected numbered parameter value4" }, result.Errors);
        }

        // todo: different types
    }
}