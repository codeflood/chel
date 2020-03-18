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

        [Fact]
        public void Bind_NamedParameterOnCommand_BindsProperty()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new NamedParameterCommand();
            var builder = new CommandInput.Builder(1, "nam");
            builder.AddNamedParameter("param1", "value1");
            var input = builder.Build();
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("value1", command.NamedParameter1);
        }

        [Fact]
        public void Bind_MultipleNamedParametersOnCommand_BindsProperty()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new NamedParameterCommand();
            var builder = new CommandInput.Builder(1, "nam");
            builder.AddNamedParameter("param1", "value1");
            builder.AddNamedParameter("param2", "value2");
            var input = builder.Build();
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("value1", command.NamedParameter1);
            Assert.Equal("value2", command.NamedParameter2);
        }

        [Fact]
        public void Bind_NamedParameterNotOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new NamedParameterCommand();
            var builder = new CommandInput.Builder(1, "nam");
            builder.AddNamedParameter("invalid", "value");
            var input = builder.Build();

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unexpected named parameter invalid" }, result.Errors);
        }

        [Fact]
        public void Bind_NoSetterOnProperty_ThrowsException()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new ParameterNoSetterCommand();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("parameter");
            var input = builder.Build();

            Action sutAction = () => sut.Bind(command, input);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Equal("Property NoSet on command type Chel.UnitTests.SampleCommands.ParameterNoSetterCommand requires a setter", ex.Message);
        }

        [Fact]
        public void Bind_MissingRequiredNumberedParameter_ErrorIncludedInResult()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new RequiredParameterCommand();
            var builder = new CommandInput.Builder(1, "command");
            var input = builder.Build();

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Missing required numbered parameter param" }, result.Errors);
        }

        [Fact]
        public void Bind_MissingRequiredNamedParameter_ThrowsException()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new RequiredNamedParameterCommand();
            var builder = new CommandInput.Builder(1, "command");
            var input = builder.Build();

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Missing required named parameter param" }, result.Errors);
        }

        [Fact]
        public void Bind_RequiredParameterProvided_PropertyIsSet()
        {
            // arrange
            var sut = new CommandParameterBinder();
            var command = new RequiredParameterCommand();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("val");
            var input = builder.Build();

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("val", command.NumberedParameter);
        }

        // todo: different types
    }
}