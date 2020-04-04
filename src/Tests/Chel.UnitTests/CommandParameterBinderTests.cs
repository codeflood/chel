using System;
using Chel.Abstractions;
using Chel.UnitTests.SampleCommands;
using Xunit;

namespace Chel.UnitTests
{
    public class CommandParameterBinderTests
    {
        [Fact]
        public void Ctor_CommandRegistryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandParameterBinder(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandRegistry", ex.ParamName);
        }

        [Fact]
        public void Bind_InstanceIsNull_ThrowsException()
        {
            // arrange
            var sut = CreateCommandParameterBinder();
            var input = CreateCommandInput("command");
            Action sutAction = () => sut.Bind(null, input);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("instance", ex.ParamName);
        }

        [Fact]
        public void Bind_InputIsNull_ThrowsException()
        {
            // arrange
            var sut = CreateCommandParameterBinder();
            var command = new SampleCommand();
            Action sutAction = () => sut.Bind(command, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("input", ex.ParamName);
        }

        [Fact]
        public void Bind_CommandNotInRegistry_ThrowsException()
        {
            // assert
            var sut = CreateCommandParameterBinder();
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num");
            Action sutAction = () => sut.Bind(command, input);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Descriptor for command num could not be resolved", ex.Message);
        }

        [Fact]
        public void Bind_NumberedParameter1StringProperty_PropertyIsSet()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", "value");

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
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", "value1", "value2");

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
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", "value1", "value2", "value3");

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
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", "value1", "value2", "value3", "value4");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unexpected numbered parameter value3", "Unexpected numbered parameter value4" }, result.Errors);
        }

        [Fact]
        public void Bind_NumberedParameterStartsWithEscapedDash_PropertyIsSet()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", @"\-value");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("-value", command.NumberedParameter1);
        }

        [Fact]
        public void Bind_NumberedParameterStartsWithDash_TreatParameterAsUnknownFlag()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", "-value");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unknown flag parameter value" }, result.Errors);
        }

        [Fact]
        public void Bind_NumberedParameterContainsDash_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", "val-ue");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("val-ue", command.NumberedParameter1);
        }

        [Fact]
        public void Bind_NamedParameterOnCommand_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-param1", "value1");
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("value1", command.NamedParameter1);
        }

        [Fact]
        public void Bind_NamedParameterDifferentCasing_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-PARAM1", "value1");
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("value1", command.NamedParameter1);
        }

        [Fact]
        public void Bind_NamedParameterMissingValue_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-param1");
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Missing value for named parameter param1" }, result.Errors);
        }

        [Fact]
        public void Bind_NamedParameterValueStartsWithEscapedDash_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-param1", @"\-value1");
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("-value1", command.NamedParameter1);
        }

        [Fact]
        public void Bind_NamedParameterValueStartsWithDash_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-param1", "-value1");
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Missing value for named parameter param1", "Unknown flag parameter value1" }, result.Errors);
        }

        [Fact]
        public void Bind_MultipleNamedParametersOnCommand_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-param1", "value1", "-param2", "value2");
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("value1", command.NamedParameter1);
            Assert.Equal("value2", command.NamedParameter2);
        }

        [Fact]
        public void Bind_MultipleNamedParametersDifferentCasing_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-ParaM1", "value1", "-ParaM2", "value2");
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("value1", command.NamedParameter1);
            Assert.Equal("value2", command.NamedParameter2);
        }

        [Fact]
        public void Bind_DuplicateNamedParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-param1", "value1", "-param1", "value2");
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat named parameter param1" }, result.Errors);
        }

        [Fact]
        public void Bind_DoubleDuplicateNamedParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-param1", "value1", "-param1", "value2", "-param1", "value3");
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat named parameter param1" }, result.Errors);
        }

        [Fact]
        public void Bind_TripleDuplicateNamedParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-param1", "value1", "-param1", "value2", "-param1", "value3", "-param1", "value4");
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat named parameter param1" }, result.Errors);
        }

        [Fact]
        public void Bind_UnknownNamedParameter_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", "-invalid", "value");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unknown named parameter invalid" }, result.Errors);
        }

        [Fact]
        public void Bind_FlagParameter_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput("command", "-p1", "-p2");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.True(command.Param1);
            Assert.True(command.Param2);
        }

        [Fact]
        public void Bind_UnknownFlagParameter_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput("command", "-unknown1", "-unknown2");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unknown flag parameter unknown2", "Unknown flag parameter unknown1" }, result.Errors);
        }

        [Fact]
        public void Bind_DuplicateFlagParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput("command", "-p1", "-p1");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat flag parameter p1" }, result.Errors);
        }

        [Fact]
        public void Bind_DoubleDuplicateFlagParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput("command", "-p1", "-p1", "-p1");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat flag parameter p1" }, result.Errors);
        }

        [Fact]
        public void Bind_TripleDuplicateFlagParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput("command", "-p1", "-p1", "-p1", "-p1");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat flag parameter p1" }, result.Errors);
        }

        [Fact]
        public void Bind_NoSetterOnProperty_ThrowsException()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterNoSetterCommand));
            var command = new ParameterNoSetterCommand();
            var input = CreateCommandInput("command", "parameter");

            Action sutAction = () => sut.Bind(command, input);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Equal("Property NoSet on command type Chel.UnitTests.SampleCommands.ParameterNoSetterCommand requires a setter", ex.Message);
        }

        [Fact]
        public void Bind_MissingRequiredNumberedParameter_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(RequiredParameterCommand));
            var command = new RequiredParameterCommand();
            var input = CreateCommandInput("command");

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
            var sut = CreateCommandParameterBinder(typeof(RequiredNamedParameterCommand));
            var command = new RequiredNamedParameterCommand();
            var input = CreateCommandInput("command");

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
            var sut = CreateCommandParameterBinder(typeof(RequiredParameterCommand));
            var command = new RequiredParameterCommand();
            var input = CreateCommandInput("command", "val");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("val", command.NumberedParameter);
        }

        [InlineData("num1", "-named", "value1")]
        [InlineData("-named", "value1", "num1")]
        [Theory]
        public void Bind_NumberedAndNamedParameters_BindsProperty(string parameter1, string parameter2, string parameter3)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(AllParametersCommand));
            var command = new AllParametersCommand();
            var input = CreateCommandInput("☕", parameter1, parameter2, parameter3);
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("num1", command.NumberedParameter);
            Assert.Equal("value1", command.NamedParameter);
        }

        [InlineData("num1", "-flag")]
        [InlineData("-flag", "num1")]
        [Theory]
        public void Bind_NumberedAndFlagParameters_BindsProperty(string parameter1, string parameter2)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(AllParametersCommand));
            var command = new AllParametersCommand();
            var input = CreateCommandInput("☕", parameter1, parameter2);
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("num1", command.NumberedParameter);
            Assert.True(command.FlagParameter);
        }

        [InlineData("-named", "value1", "-flag")]
        [InlineData("-flag", "-named", "value1")]
        [Theory]
        public void Bind_NamedAndFlagParameters_BindsProperty(string parameter1, string parameter2, string parameter3)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(AllParametersCommand));
            var command = new AllParametersCommand();
            var input = CreateCommandInput("☕", parameter1, parameter2, parameter3);
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("value1", command.NamedParameter);
            Assert.True(command.FlagParameter);
        }

        [InlineData("-flag", "-named", "value1", "num1")]
        [InlineData("-flag", "num1", "-named", "value1")]
        [InlineData("-named", "value1", "num1", "-flag")]
        [InlineData("-named", "value1", "-flag", "num1")]
        [InlineData("num1", "-flag", "-named", "value1")]
        [InlineData("num1", "-named", "value1", "-flag")]
        [Theory]
        public void Bind_AllParameters_BindsProperty(string parameter1, string parameter2, string parameter3, string parameter4)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(AllParametersCommand));
            var command = new AllParametersCommand();
            var input = CreateCommandInput("☕", parameter1, parameter2, parameter3, parameter4);
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.True(command.FlagParameter);
            Assert.Equal("num1", command.NumberedParameter);
            Assert.Equal("value1", command.NamedParameter);
        }

        private CommandParameterBinder CreateCommandParameterBinder(params Type[] commandTypes)
        {
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);

            foreach(var commandType in commandTypes)
                registry.Register(commandType);

            return new CommandParameterBinder(registry);
        }

        private CommandInput CreateCommandInput(string commandName, params string[] parameters)
        {
            var builder = new CommandInput.Builder(1, commandName);

            foreach(var parameter in parameters)
                builder.AddParameter(parameter);
            
            return builder.Build();
        }

        // todo: different types
    }
}