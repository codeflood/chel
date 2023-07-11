using System;
using System.Collections.Generic;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
using Chel.Abstractions.UnitTests.SampleCommands;
using Chel.Abstractions.Variables;
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
            Action sutAction = () => new CommandParameterBinder(null, new VariableReplacer(), new VariableCollection());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandRegistry", ex.ParamName);
        }

        [Fact]
        public void Ctor_VariableReplacerIsNull_ThrowsException()
        {
            // arrange
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);
            Action sutAction = () => new CommandParameterBinder(registry, null, new VariableCollection());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variableReplacer", ex.ParamName);
        }

        [Fact]
        public void Ctor_VariablesIsNull_ThrowsException()
        {
            // arrange
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);
            Action sutAction = () => new CommandParameterBinder(registry, new VariableReplacer(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variables", ex.ParamName);
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
            Assert.Contains("Descriptor for command 'num' could not be resolved", ex.Message);
        }

        [Fact]
        public void Bind_NumberedParameter1StringProperty_PropertyIsSet()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", new Literal("value"));

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
            var input = CreateCommandInput("num", new Literal("value1"), new Literal("value2"));

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
            var input = CreateCommandInput(
                "num",
                new SourceValueCommandParameter(new(1, 1), new Literal("value1")),
                new SourceValueCommandParameter(new(1, 2), new Literal("value2")),
                new SourceValueCommandParameter(new(1, 3), new Literal("value3"))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 3), "Unexpected numbered parameter 'value3'") }, result.Errors);
        }

        [Fact]
        public void Bind_TwoTooManyNumberedParametersProvided_ErrorsIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput(
                "num",
                new SourceValueCommandParameter(new(1, 1), new Literal("value1")),
                new SourceValueCommandParameter(new(1, 2), new Literal("value2")),
                new SourceValueCommandParameter(new(1, 3), new Literal("value3")),
                new SourceValueCommandParameter(new(1, 4), new Literal("value4"))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[] {
                new SourceError(new(1, 3), "Unexpected numbered parameter 'value3'"),
                new SourceError(new(1, 4), "Unexpected numbered parameter 'value4'")
            }, result.Errors);
        }

        [Fact]
        public void Bind_NumberedParameterStartsWithEscapedDash_PropertyIsSet()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", new Literal("-value"));

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("-value", command.NumberedParameter1);
        }

        [Fact]
        public void Bind_NumberedParameterContainsDash_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", new Literal("val-ue"));

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("val-ue", command.NumberedParameter1);
        }

        [Fact]
        public void Bind_NumberedParameterInvalidValue_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumericNumberedParameterCommand));
            var command = new NumericNumberedParameterCommand();
            var input = CreateCommandInput(
                "command",
                new SourceValueCommandParameter(new(1, 3), new Literal("3000000000"))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Collection(result.Errors, x => x.Message.StartsWith("Invalid parameter value '3000000000' for numbered parameter 'num'."));
        }

        [Fact]
        public void Bind_NamedParameterOnCommand_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 5), "param1"),
                new Literal("value1")
            );
            
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
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 5), "PARAM1"),
                new Literal("value1")
            );
            
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
            var input = CreateCommandInput("nam", new ParameterNameCommandParameter(new(1, 5), "param1"));
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 5), "Missing value for named parameter 'param1'") }, result.Errors);
        }

        [Fact]
        public void Bind_NamedParameterValueStartsWithDash_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", new ParameterNameCommandParameter(new(1, 5), "param1"), new Literal("-value1"));
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("-value1", command.NamedParameter1);
        }

        [Fact]
        public void Bind_MultipleNamedParametersOnCommand_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 5), "param1"),
                new Literal("value1"),
                new ParameterNameCommandParameter(new(1, 19), "param2"),
                new Literal("value2")
            );
            
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
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 5), "ParaM1"),
                new Literal("value1"),
                new ParameterNameCommandParameter(new(1, 19), "ParaM2"),
                new Literal("value2")
            );
            
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
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 5), "param1"),
                new Literal("value1"),
                new ParameterNameCommandParameter(new(1, 19), "param1"),
                new Literal("value2")
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 19), "Cannot repeat named parameter 'param1'") }, result.Errors);
        }

        [Fact]
        public void Bind_DoubleDuplicateNamedParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 5), "param1"),
                new Literal("value1"),
                new ParameterNameCommandParameter(new(1, 19), "param1"),
                new Literal("value2"),
                new ParameterNameCommandParameter(new(1, 33), "param1"),
                new Literal("value3")
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{
                new SourceError(new(1, 19), "Cannot repeat named parameter 'param1'"),
                new SourceError(new(1, 33), "Cannot repeat named parameter 'param1'")
            }, result.Errors);
        }

        [Fact]
        public void Bind_TripleDuplicateNamedParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 5), "param1"),
                new Literal("value1"),
                new ParameterNameCommandParameter(new(1, 19), "param1"),
                new Literal("value2"),
                new ParameterNameCommandParameter(new(1, 33), "param1"),
                new Literal("value3"),
                new ParameterNameCommandParameter(new(1, 57), "param1"),
                new Literal("value4")
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{
                new SourceError(new(1, 19), "Cannot repeat named parameter 'param1'"),
                new SourceError(new(1, 33), "Cannot repeat named parameter 'param1'"),
                new SourceError(new(1, 57), "Cannot repeat named parameter 'param1'")
            }, result.Errors);
        }

        [Fact]
        public void Bind_UnknownNamedParameter_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 5), "invalid"),
                new Literal("value")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 5), "Unknown named parameter 'invalid'") }, result.Errors);
        }

        [Fact]
        public void Bind_FlagParameter_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9) ,"p1"),
                new ParameterNameCommandParameter(new(1, 12), "p2")
            );

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
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "unknown1"),
                new ParameterNameCommandParameter(new(1, 13), "unknown2")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{
                new SourceError(new(1, 13), "Unknown flag parameter 'unknown2'"),
                new SourceError(new(1, 9), "Unknown flag parameter 'unknown1'")
            }, result.Errors);
        }

        [Fact]
        public void Bind_DuplicateFlagParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 1), "p1"),
                new ParameterNameCommandParameter(new(1, 2), "p1")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 2), "Cannot repeat flag parameter 'p1'") }, result.Errors);
        }

        [Fact]
        public void Bind_DoubleDuplicateFlagParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 1), "p1"),
                new ParameterNameCommandParameter(new(1, 2), "p1"),
                new ParameterNameCommandParameter(new(1, 3), "p1")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{
                new SourceError(new(1, 2), "Cannot repeat flag parameter 'p1'"),
                new SourceError(new(1, 3), "Cannot repeat flag parameter 'p1'")
            }, result.Errors);
        }

        [Fact]
        public void Bind_TripleDuplicateFlagParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 1), "p1"),
                new ParameterNameCommandParameter(new(1, 2), "p1"),
                new ParameterNameCommandParameter(new(1, 3), "p1"),
                new ParameterNameCommandParameter(new(1, 4), "p1")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{
                new SourceError(new(1, 2), "Cannot repeat flag parameter 'p1'"),
                new SourceError(new(1, 3), "Cannot repeat flag parameter 'p1'"),
                new SourceError(new(1, 4), "Cannot repeat flag parameter 'p1'")
            }, result.Errors);
        }

        [Fact]
        public void Bind_NoSetterOnProperty_ThrowsException()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterNoSetterCommand));
            var command = new ParameterNoSetterCommand();
            var input = CreateCommandInput("command", new Literal("parameter"));

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
            Assert.Equal(new[]{new SourceError(new(1, 1), "Missing required numbered parameter 'param'") }, result.Errors);
        }

        [Fact]
        public void Bind_MissingRequiredNamedParameter_ReturnsError()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(RequiredNamedParameterCommand));
            var command = new RequiredNamedParameterCommand();
            var input = CreateCommandInput("command");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 1), "Missing required named parameter 'param'") }, result.Errors);
        }

        [Fact]
        public void Bind_RequiredParameterProvided_PropertyIsSet()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(RequiredParameterCommand));
            var command = new RequiredParameterCommand();
            var input = CreateCommandInput("command", new Literal("val"));

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("val", command.NumberedParameter);
        }

        [Theory]
        [MemberData(nameof(Bind_NumberedAndNamedParameters_BindsProperty_Datasource))]
        public void Bind_NumberedAndNamedParameters_BindsProperty(ICommandParameter parameter1, ICommandParameter parameter2, ICommandParameter parameter3)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(AllParametersCommand));
            var command = new AllParametersCommand();
            var input = CreateCommandInput("☕", parameter1, parameter2, parameter3
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("num1", command.NumberedParameter);
            Assert.Equal("value1", command.NamedParameter);
        }

        public static IEnumerable<object[]> Bind_NumberedAndNamedParameters_BindsProperty_Datasource()
        {
            yield return new object[] {
                new SourceValueCommandParameter(new(1, 1), new Literal("num1")),
                new ParameterNameCommandParameter(new(1, 2), "named"),
                new SourceValueCommandParameter(new(1, 3), new Literal("value1"))
            };

            yield return new object[] {
                new ParameterNameCommandParameter(new(1, 1), "named"),
                new SourceValueCommandParameter(new(1, 2), new Literal("value1")),
                new SourceValueCommandParameter(new(1, 3), new Literal("num1"))
            };
        }

        [Theory]
        [MemberData(nameof(Bind_NumberedAndFlagParameters_BindsProperty_Datasource))]
        public void Bind_NumberedAndFlagParameters_BindsProperty(ICommandParameter parameter1, ICommandParameter parameter2)
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

        public static IEnumerable<object[]> Bind_NumberedAndFlagParameters_BindsProperty_Datasource()
        {
            yield return new object[] {
                new SourceValueCommandParameter(new(1, 1), new Literal("num1")),
                new ParameterNameCommandParameter(new(1, 2), "flag")
            };

            yield return new object[] {
                new ParameterNameCommandParameter(new(1, 1), "flag"),
                new SourceValueCommandParameter(new(1, 2), new Literal("num1")),
            };
        }

        [Theory]
        [MemberData(nameof(Bind_NamedAndFlagParameters_BindsProperty_Datasource))]
        public void Bind_NamedAndFlagParameters_BindsProperty(ICommandParameter parameter1, ICommandParameter parameter2, ICommandParameter parameter3)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(AllParametersCommand));
            var command = new AllParametersCommand();
            var input = CreateCommandInput("☕", parameter1, parameter2, parameter3
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("value1", command.NamedParameter);
            Assert.True(command.FlagParameter);
        }

        public static IEnumerable<object[]> Bind_NamedAndFlagParameters_BindsProperty_Datasource()
        {
            yield return new object[] {
                new ParameterNameCommandParameter(new(1, 1), "named"),
                new SourceValueCommandParameter(new(1, 2), new Literal("value1")),
                new ParameterNameCommandParameter(new(1, 3), "flag")
            };

            yield return new object[] {
                new ParameterNameCommandParameter(new(1, 1), "flag"),
                new ParameterNameCommandParameter(new(1, 2), "named"),
                new SourceValueCommandParameter(new(1, 3), new Literal("value1"))
            };
        }

        [Theory]
        [MemberData(nameof(Bind_AllParameters_BindsProperty_Datasource))]
        public void Bind_AllParameters_BindsProperty(
            ICommandParameter parameter1,
            ICommandParameter parameter2,
            ICommandParameter parameter3,
            ICommandParameter parameter4)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(AllParametersCommand));
            var command = new AllParametersCommand();
            var input = CreateCommandInput("☕", parameter1, parameter2, parameter3, parameter4
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.True(command.FlagParameter);
            Assert.Equal("num1", command.NumberedParameter);
            Assert.Equal("value1", command.NamedParameter);
        }

        public static IEnumerable<object[]> Bind_AllParameters_BindsProperty_Datasource()
        {
            yield return new object[] {
                new ParameterNameCommandParameter(new(1, 1), "flag"),
                new ParameterNameCommandParameter(new(1, 2), "named"),
                new SourceValueCommandParameter(new(1, 3), new Literal("value1")),
                new SourceValueCommandParameter(new(1, 4), new Literal("num1"))
            };

            yield return new object[] {
                new ParameterNameCommandParameter(new(1, 1), "flag"),
                new SourceValueCommandParameter(new(1, 2), new Literal("num1")),
                new ParameterNameCommandParameter(new(1, 3), "named"),
                new SourceValueCommandParameter(new(1, 4), new Literal("value1"))
            };

            yield return new object[] {
                new ParameterNameCommandParameter(new(1, 1), "named"),
                new SourceValueCommandParameter(new(1, 2), new Literal("value1")),
                new ParameterNameCommandParameter(new(1, 3), "flag"),
                new SourceValueCommandParameter(new(1, 4), new Literal("num1"))
            };

            yield return new object[] {
                new ParameterNameCommandParameter(new(1, 1), "named"),
                new SourceValueCommandParameter(new(1, 2), new Literal("value1")),
                new SourceValueCommandParameter(new(1, 3), new Literal("num1")),
                new ParameterNameCommandParameter(new(1, 4), "flag")
            };

            yield return new object[] {
                new SourceValueCommandParameter(new(1, 1), new Literal("num1")),
                new ParameterNameCommandParameter(new(1, 2), "named"),
                new SourceValueCommandParameter(new(1, 3), new Literal("value1")),
                new ParameterNameCommandParameter(new(1, 4), "flag")
            };

            yield return new object[] {
                new SourceValueCommandParameter(new(1, 1), new Literal("num1")),
                new ParameterNameCommandParameter(new(1, 2), "flag"),
                new ParameterNameCommandParameter(new(1, 3), "named"),
                new SourceValueCommandParameter(new(1, 4), new Literal("value1"))
            };
        }

        [InlineData("True", true)]
        [InlineData("true", true)]
        [InlineData("TRUE", true)]
        [InlineData("False", false)]
        [InlineData("false", false)]
        [InlineData("FALSE", false)]
        [Theory]
        public void Bind_BoolTypeParameter_BindsParameter(string value, bool expected)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "bool"),
                new SourceValueCommandParameter(new(1, 15), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Bool);
        }

        [InlineData("0", 0x0)]
        [InlineData("1", 0x1)]
        [InlineData("4", 0x4)]
        [InlineData("15", 0xf)]
        [InlineData("0xf", 0xf)]
        [InlineData("255", 0xff)]
        [InlineData("0xff", 0xff)]
        [Theory]
        public void Bind_ByteTypeParameter_BindsParameter(string value, byte expected)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "byte"),
                new SourceValueCommandParameter(new(1, 15), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Byte);
        }

        [InlineData("Value1", SampleEnum.Value1)]
        [InlineData("Value2", SampleEnum.Value2)]
        [InlineData("value2", SampleEnum.Value2)]
        [InlineData("VALUE2", SampleEnum.Value2)]
        [Theory]
        public void Bind_EnumTypeParameter_BindsParameter(string value, SampleEnum expected)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "enum"),
                new SourceValueCommandParameter(new(1, 15), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Enum);
        }

        [InlineData("0", 0)]
        [InlineData("1", 1)]
        [InlineData("1000020", 1000020)]
        [InlineData("-2147483648", int.MinValue)]
        [InlineData("2147483647", int.MaxValue)]
        [Theory]
        public void Bind_IntTypeParameter_BindsParameter(string value, int expected)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "int"),
                new SourceValueCommandParameter(new(1, 14), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Int);
        }

        [InlineData("0", '0')]
        [InlineData("Z", 'Z')]
        [InlineData("+", '+')]
        [Theory]
        public void Bind_CharTypeParameter_BindsParameter(string value, char expected)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "char"),
                new SourceValueCommandParameter(new(1, 15), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Char);
        }

        [InlineData("0.234", 0.234)]
        [InlineData("17.98235", 17.98235)]
        [InlineData("62534486.927846453", 62534486.927846453)]
        [Theory]
        public void Bind_FloatTypeParameter_BindsParameter(string value, float expected)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "float"),
                new SourceValueCommandParameter(new(1, 16), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Float);
        }

        [InlineData("0.1234567890", 0.1234567890)]
        [InlineData("1234567890.1234567890", 1234567890.1234567890)]
        [Theory]
        public void Bind_DoubleTypeParameter_BindsParameter(string value, double expected)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "double"),
                new SourceValueCommandParameter(new(1, 17), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Double);
        }

        [MemberData(nameof(Bind_DateTypeParameter_BindsParameter_Data))]
        [Theory]
        public void Bind_DateTypeParameter_BindsParameter(string value, DateTime expected)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "date"),
                new SourceValueCommandParameter(new(1, 15), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Date);
        }

        public static IEnumerable<object[]> Bind_DateTypeParameter_BindsParameter_Data()
        {
            yield return new object[] { "1900-01-01", new DateTime(1900, 1, 1) };
            yield return new object[] { "1900-01-01T13:12:11", new DateTime(1900, 1, 1, 13, 12, 11) };
            yield return new object[] { "2578-12-31", new DateTime(2578, 12, 31) };
            yield return new object[] { "2578-12-31T23:45:45", new DateTime(2578, 12, 31, 23, 45, 45) };
        }

        [MemberData(nameof(Bind_TimeSpanTypeParameter_BindsParameter_Data))]
        [Theory]
        public void Bind_TimeSpanTypeParameter_BindsParameter(string value, TimeSpan expected)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "time"),
                new SourceValueCommandParameter(new(1, 15), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Time);
        }

        public static IEnumerable<object[]> Bind_TimeSpanTypeParameter_BindsParameter_Data()
        {
            yield return new object[] { "00:00:00.003", TimeSpan.FromMilliseconds(3) };
            yield return new object[] { "00:00:00", TimeSpan.Zero };
            yield return new object[] { "00:00:01", TimeSpan.FromSeconds(1) };
            yield return new object[] { "23:27:27.27", new TimeSpan(0, 23, 27, 27, 270) };
            yield return new object[] { "27.23:27:27.27", new TimeSpan(27, 23, 27, 27, 270) };
        }

        [MemberData(nameof(Bind_GuidTypeParameter_BindsParameter_Data))]
        [Theory]
        public void Bind_GuidTypeParameter_BindsParameter(string value, Guid expected)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "guid"),
                new SourceValueCommandParameter(new(1, 15), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Guid);
        }

        public static IEnumerable<object[]> Bind_GuidTypeParameter_BindsParameter_Data()
        {
            yield return new object[] { "00000000-0000-0000-0000-000000000000", new Guid("00000000-0000-0000-0000-000000000000") };
            yield return new object[] { "ffffffff-ffff-ffff-ffff-ffffffffffff", new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff") };
            yield return new object[] { "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF") };
            yield return new object[] { "12341234-1234-1234-1234-123412341234", new Guid("12341234-1234-1234-1234-123412341234") };
            yield return new object[] { "{12341234-1234-1234-1234-123412341234}", new Guid("12341234-1234-1234-1234-123412341234") };
            yield return new object[] { "12341234123412341234123412341234", new Guid("12341234-1234-1234-1234-123412341234") };
        }

        [MemberData(nameof(Bind_GuidTypeParameterInvalidValue_ErrorIncludedInResult_Data))]
        [Theory]
        public void Bind_GuidTypeParameterInvalidValue_ErrorIncludedInResult(string value)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "guid"),
                new SourceValueCommandParameter(new(1, 15), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.StartsWith($"Invalid parameter value '{value}' for named parameter 'guid'.", result.Errors[0].Message);
        }

        public static IEnumerable<object[]> Bind_GuidTypeParameterInvalidValue_ErrorIncludedInResult_Data()
        {
            yield return new object[] { "000000-0000-0000-0000-000000000000" };
            yield return new object[] { "ffffzfff-ffff-ffff-ffff-ffffffffffff" };
        }

        [InlineData("byte", "z")]
        [InlineData("byte", "1111")]
        [InlineData("char", "aa")]
        [InlineData("int", "abc")]
        [InlineData("float", "abc")]
        [InlineData("double", "abc")]
        [InlineData("date", "1984-13-42")]
        [InlineData("date", "1984-10-10T27:75:75")]
        [InlineData("time", "abc")]
        [InlineData("guid", "abc")]
        [Theory]
        public void Bind_InvalidValue_ErrorIncludedInResult(string parameter, string value)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), parameter),
                new SourceValueCommandParameter(new(1, 15), new Literal(value))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.StartsWith($"Invalid parameter value '{value}' for named parameter '{parameter}'.", result.Errors[0].Message);
        }

        [Fact]
        public void Bind_PropertyHasTypeConverter_BindsParameter()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 9), "complex"),
                new SourceValueCommandParameter(new(1, 15), new Literal("name:42"))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("name", command.ComplexType.Name);
            Assert.Equal(42, command.ComplexType.Number);
        }

        [Fact]
        public void Bind_NamedParameterContainsVariable_VariableIsReplaced()
        {
            // arrange
            var registry = CreateCommandRegistry(typeof(NamedParameterCommand));
            var variables = new VariableCollection();
            variables.Set(new Variable("foo", new Literal("bar")));

            var replacer = new VariableReplacer();

            var sut = new CommandParameterBinder(registry, replacer, variables);
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 1), "param1"),
                new SourceValueCommandParameter(new(1, 15), new VariableReference("foo"))
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("bar", command.NamedParameter1);
        }

        [Fact]
        public void Bind_NamedParameterContainsUnsetVariable_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 5), "param1"),
                new SourceValueCommandParameter(new(1, 15), new VariableReference("foo"))
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 15), "Variable $foo$ is not set") }, result.Errors);
        }

        [Fact]
        public void Bind_NamedParameterContainsInvalidVariableSubreference_ErrorIncludedInResult()
        {
            // arrange
            var registry = CreateCommandRegistry(typeof(ListParameterCommand));
            var variables = new VariableCollection();
            variables.Set(new Variable("foo", new List(new[]{ new Literal("bar") })));

            var replacer = new VariableReplacer();

            var sut = new CommandParameterBinder(registry, replacer, variables);
            var command = new ListParameterCommand();
            var input = CreateCommandInput("list-params",
                new ParameterNameCommandParameter(new(1, 9), "list"),
                new SourceValueCommandParameter(new(1, 15), new VariableReference("foo", new[] {"200"}))
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 15), "Variable $foo$ subreference '200' is invalid") }, result.Errors);
        }

        [Fact]
        public void Bind_NamedParameterContainsInvalidVariableSubSubreference_ErrorIncludedInResult()
        {
            // arrange
            var registry = CreateCommandRegistry(typeof(ListParameterCommand));
            var variables = new VariableCollection();
            variables.Set(new Variable("foo", new List(new[]{ new Literal("bar") })));

            var replacer = new VariableReplacer();

            var sut = new CommandParameterBinder(registry, replacer, variables);
            var command = new ListParameterCommand();
            var input = CreateCommandInput("list-params",
                new ParameterNameCommandParameter(new(1, 9), "list"),
                new SourceValueCommandParameter(new(1, 15), new VariableReference("foo", new[] { "1", "2" }))
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 15), "Variable $foo:1$ subreference '2' is invalid") }, result.Errors);
        }

        [Fact]
        public void Bind_NumberedParameterContainsVariable_VariableIsReplaced()
        {
            // arrange
            var registry = CreateCommandRegistry(typeof(NumericNumberedParameterCommand));
            var variables = new VariableCollection();
            variables.Set(new Variable("foo", new Literal("10")));

            var replacer = new VariableReplacer();

            var sut = new CommandParameterBinder(registry, replacer, variables);
            var command = new NumericNumberedParameterCommand();
            var input = CreateCommandInput(
                "command",
                new SourceValueCommandParameter(new(1, 9), new VariableReference("foo"))
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(10, command.NumberedParameter);
        }

        [Fact]
        public void Bind_NumberedParameterContainsUnsetVariable_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumericNumberedParameterCommand));
            var command = new NumericNumberedParameterCommand();
            var input = CreateCommandInput(
                "command",
                new SourceValueCommandParameter(new(1, 9), new VariableReference("foo"))
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 9), "Variable $foo$ is not set") }, result.Errors);
        }

        [Theory]
        [MemberData(nameof(Bind_SetList_BindsParameter_DataSource))]
        public void Bind_SetList_BindsParameter(string parameterName, Func<ListParameterCommand, IEnumerable<string>> propertyExtractor)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ListParameterCommand));
            var command = new ListParameterCommand();
            var input = CreateCommandInput(
                "list-params",
                new ParameterNameCommandParameter(new(1, 9), parameterName),
                new SourceValueCommandParameter(new(1, 13),
                    new List(new[]
                    {
                        new Literal("a"),
                        new Literal("b")
                    })
                )
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            var values = propertyExtractor(command);
            Assert.Equal(new[] { "a", "b" }, values);
        }

        public static IEnumerable<object[]> Bind_SetList_BindsParameter_DataSource()
        {
            yield return new object[] { "array", new Func<ListParameterCommand, IEnumerable<string>>(command => command.Array) };
            yield return new object[] { "enumerable", new Func<ListParameterCommand, IEnumerable<string>>(command => command.Enumerable) };
            yield return new object[] { "list", new Func<ListParameterCommand, IEnumerable<string>>(command => command.List) };
            yield return new object[] { "collection", new Func<ListParameterCommand, IEnumerable<string>>(command => command.Collection) };
            yield return new object[] { "rocollection", new Func<ListParameterCommand, IEnumerable<string>>(command => command.ReadOnlyCollection) };
            yield return new object[] { "concretelist", new Func<ListParameterCommand, IEnumerable<string>>(command => command.ConcreteList) };

        }

        [Fact]
        public void Bind_SetListOnString_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter(new(1, 1), "param1"),
                new SourceValueCommandParameter(new(2, 3), new List(new[]
                {
                    new Literal("a"),
                    new Literal("b")
                }))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(2, 3), "Cannot bind a list to a non-list parameter 'param1'") }, result.Errors);
        }

        [Fact]
        public void Bind_SetListOnIncompatibleEnumerable_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ListParameterCommand));
            var command = new ListParameterCommand();
            var input = CreateCommandInput(
                "list-params",
                new ParameterNameCommandParameter(new(1, 1), "intlist"),
                new SourceValueCommandParameter(new(2, 3), new List(new[]
                {
                    new Literal("a"),
                    new Literal("b")
                }))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count);
            Assert.Equal(new SourceLocation(2, 3), result.Errors[0].SourceLocation);
            Assert.StartsWith("Invalid parameter value '[ a b ]' for named parameter 'intlist'.", result.Errors[0].Message);
        }

        [Fact]
        public void Bind_SetLiteralOnList_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ListParameterCommand));
            var command = new ListParameterCommand();
            var input = CreateCommandInput(
                "list-params",
                new ParameterNameCommandParameter(new(1, 1), "list"),
                new SourceValueCommandParameter(new(2, 3), new Literal("a"))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(2, 3), "Cannot bind a non-list to a list parameter 'list'")}, result.Errors);
        }

        [Fact]
        public void Bind_VariableInList_SubstitutesVariableValue()
        {
            // arrange
            var registry = CreateCommandRegistry(typeof(ListParameterCommand));
            var variables = new VariableCollection();
            variables.Set(new Variable("foo", new Literal("bar")));

            var replacer = new VariableReplacer();

            var sut = new CommandParameterBinder(registry, replacer, variables);

            var command = new ListParameterCommand();
            var input = CreateCommandInput(
                "list-params",
                new ParameterNameCommandParameter(new(1, 1), "list"),
                new List(new ChelType[]
                {
                    new Literal("a"),
                    new VariableReference("foo")
                })
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(new[] { "a", "bar" }, command.List);
        }

        [Fact]
        public void Bind_VariableIsList_BindsParameter()
        {
            // arrange
            var registry = CreateCommandRegistry(typeof(ListParameterCommand));
            var variables = new VariableCollection();
            variables.Set(new Variable("foo", new List(new ChelType[] {
                new Literal("val1"),
                new Literal("val2")
            })));

            var replacer = new VariableReplacer();

            var sut = new CommandParameterBinder(registry, replacer, variables);

            var command = new ListParameterCommand();
            var input = CreateCommandInput(
                "list-params",
                new ParameterNameCommandParameter(new(1, 1), "list"),
                new VariableReference("foo")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(new[] { "val1", "val2" }, command.List);
        }

        [Theory]
        [MemberData(nameof(Bind_PropertyIsChelType_BindsParameter_DataSource))]
        public void Bind_PropertyIsChelType_BindsParameter(ChelType value)
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(Var));
            var variables = new VariableCollection();
            var command = new Var(variables, new PhraseDictionary(), new NameValidator());
            var input = CreateCommandInput(
                "var",
                new Literal("foo"),
                value
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(value, command.Value);
        }

        public static IEnumerable<object[]> Bind_PropertyIsChelType_BindsParameter_DataSource()
        {
            yield return new[] { new Literal("lit") };
            yield return new[] { new CompoundValue(new ChelType[] { new Literal("lit"), new Literal("lit2") }) };
            yield return new[] { new List(new ChelType[] { new Literal("lit1"), new Literal("lit2") }) };
        }

        [Theory]
        [MemberData(nameof(Bind_PropertyIsChelTypeValueHasVariableReference_VariableIsReplaced_DataSource))]
        public void Bind_PropertyIsChelTypeValueHasVariableReference_VariableIsReplaced(ChelType value, ChelType expected)
        {
            // arrange
            var variables = new VariableCollection();
            variables.Set(new Variable("ref", new Literal("val")));

            var sut = CreateCommandParameterBinder(variables, typeof(Var));

            var command = new Var(variables, new PhraseDictionary(), new NameValidator());
            var input = CreateCommandInput(
                "var",
                new Literal("foo"),
                value
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(expected, command.Value);
        }

        public static IEnumerable<object[]> Bind_PropertyIsChelTypeValueHasVariableReference_VariableIsReplaced_DataSource()
        {
            yield return new ChelType[]
            {
                new VariableReference("ref"),
                new Literal("val")
            };

            yield return new[]
            {
                new CompoundValue(new ChelType[] { new Literal("lit"), new VariableReference("ref") }),
                new CompoundValue(new ChelType[] { new Literal("lit"), new Literal("val") }),
            };

            yield return new[]
            {
                new List(new ChelType[] { new Literal("lit1"), new VariableReference("ref") }),
                new List(new ChelType[] { new Literal("lit1"), new Literal("val") })
            };
        }

        [Fact]
        public void Bind_PropertyIsChelTypeInputIsSourceValueCommandParameter_BindsValue()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(Var));
            var variables = new VariableCollection();
            var command = new Var(variables, new PhraseDictionary(), new NameValidator());
            var input = CreateCommandInput(
                "var",
                new Literal("foo"),
                new SourceValueCommandParameter(new(1, 1), new Literal("lit"))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.IsType<Literal>(command.Value);
        }

        [Fact]
        public void Bind_PropertyIsChelTypeInputIsSourceValueCommandParameterWithList_BindsListValues()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(Var));
            var variables = new VariableCollection();
            var command = new Var(variables, new PhraseDictionary(), new NameValidator());
            var input = CreateCommandInput(
                "var",
                new Literal("foo"),
                new SourceValueCommandParameter(new(1, 1), new List(new[]{
                    new SourceValueCommandParameter(new(1, 3), new Literal("lit")),
                    new SourceValueCommandParameter(new(1, 7), new Literal("lit2"))
                }))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            var list = Assert.IsType<List>(command.Value);
            Assert.IsType<Literal>(list.Values[0]);
            Assert.IsType<Literal>(list.Values[1]);
        }

        [Fact]
        public void Bind_PropertyIsChelTypeInputIsSourceValueCommandParameterWithMap_BindsMapValues()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(Var));
            var variables = new VariableCollection();
            var command = new Var(variables, new PhraseDictionary(), new NameValidator());
            var input = CreateCommandInput(
                "var",
                new Literal("foo"),
                new SourceValueCommandParameter(new(1, 1), new Map(new Dictionary<string, ICommandParameter> {
                    { "a", new SourceValueCommandParameter(new(1, 3), new Literal("lit")) },
                    { "b", new SourceValueCommandParameter(new(1, 7), new Literal("lit2")) }
                }))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            var map = Assert.IsType<Map>(command.Value);
            Assert.IsType<Literal>(map.Entries["a"]);
            Assert.IsType<Literal>(map.Entries["b"]);
        }

        [Fact]
        public void Bind_ParameterIsCommandLineWithoutSubstituteValue_ThrowsException()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var subcommand = CreateCommandInput(
                "nam", 
                new ParameterNameCommandParameter(new(1, 1), "param1"),
                new Literal("val")
            );

            var input = new CommandInput.Builder(new(1, 1), "nam");
            input.AddParameter(new ParameterNameCommandParameter(new(1, 1), "param1"));
            input.AddParameter(subcommand);

            var command = new NamedParameterCommand();

            Action sutAction = () => sut.Bind(command, input.Build());

            // act, assert
            Assert.Throws<InvalidOperationException>(sutAction);
        }

        [Fact]
        public void Bind_ParameterIsCommandLineWithSubstituteValue_UsesSubstituteValue()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var subcommand = CreateCommandInput(
                "nam", 
                new ParameterNameCommandParameter(new(1, 1), "param1"),
                new Literal("val")
            );
            subcommand.SubstituteValue = new Literal("subbed");

            var input = new CommandInput.Builder(new(1, 1), "nam");
            input.AddParameter(new ParameterNameCommandParameter(new(1, 1), "param1"));
            input.AddParameter(subcommand);

            var command = new NamedParameterCommand();

            // act
            sut.Bind(command, input.Build());

            // assert
            Assert.Equal("subbed", command.NamedParameter1);
        }

        [Fact]
        public void Bind_ListParameterContainsCommandLine_UsesSubstituteValue()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand), typeof(ListParameterCommand));
            var subcommand = CreateCommandInput(
                "nam", 
                new ParameterNameCommandParameter(new(1, 5), "param1"),
                new SourceValueCommandParameter(new(1, 13), new Literal("val"))
            );
            subcommand.SubstituteValue = new Literal("subbed");

            var commandInput = CreateCommandInput(
                "list-params",
                new ParameterNameCommandParameter(new(1, 10), "list"),
                new SourceValueCommandParameter(new(1, 16), new List(new[] { subcommand }))
            );

            var command = new ListParameterCommand();

            // act
            sut.Bind(command, commandInput);

            // assert
            Assert.Equal("subbed", command.List[0]);
        }

        [Fact]
        public void Bind_ListParameterContainsMap_BindsToDictionary()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ListParameterCommand));
            var command = new ListParameterCommand();
            var input = CreateCommandInput(
                "list-params",
                new ParameterNameCommandParameter(new(1, 1), "maplist"),
                new List(new[]
                {
                    new Map(new MapEntries
                    {
                        { "key1", new Literal("value1") },
                        { "key2", new Literal("value2") }
                    }),
                    new Map(new MapEntries
                    {
                        { "key3", new Literal("value3") },
                        { "key4", new Literal("value4") }
                    })
                })
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(2, command.MapList.Count);
            Assert.Equal("value1", ((Literal)command.MapList[0].Entries["key1"]).Value);
            Assert.Equal("value2", ((Literal)command.MapList[0].Entries["key2"]).Value);
            Assert.Equal("value3", ((Literal)command.MapList[1].Entries["key3"]).Value);
            Assert.Equal("value4", ((Literal)command.MapList[1].Entries["key4"]).Value);
        }

        [Fact]
        public void Bind_ExpandNonMapParameter_ErrorsIncludedInResult()
        {
            // arrange
            var variables = new VariableCollection();
            variables.Set(new Variable("foo", new Literal("bar")));

            var sut = CreateCommandParameterBinder(variables, typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();

            var commandInput = CreateCommandInput(
                "nam",
                new SourceValueCommandParameter(new(1, 5), new VariableReference("*foo"))
            );

            // act
            var result = sut.Bind(command, commandInput);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(1, 5), "Variable 'foo' is not a map and cannot be expanded") }, result.Errors);
        }

        [Fact]
        public void Bind_ExpandMapParameter_ParametersSet()
        {
            // arrange
            var map = new Map(new MapEntries
            {
                { "param1", new Literal("val1") },
                { "param2", new Literal("val2") }
            });
            var variables = new VariableCollection();
            variables.Set(new Variable("map", map));

            var sut = CreateCommandParameterBinder(variables, typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();

            var commandInput = CreateCommandInput(
                "nam",
                new SourceValueCommandParameter(new(1, 5), new VariableReference("*map"))
            );

            // act
            var result = sut.Bind(command, commandInput);

            // assert
            Assert.True(result.Success);
            Assert.Equal("val1", command.NamedParameter1);
            Assert.Equal("val2", command.NamedParameter2);
        }

        [Fact]
        public void Bind_ExpandMapParameterFromSubreference_ParametersSet()
        {
            // arrange
            var map = new Map(new MapEntries
            {
                { "param1", new Literal("val1") },
                { "param2", new Literal("val2") }
            });
            var list = new List(new ICommandParameter[] { new Literal("lit"), map });
            var variables = new VariableCollection();
            variables.Set(new Variable("list", list));

            var sut = CreateCommandParameterBinder(variables, typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();

            var commandInput = CreateCommandInput(
                "nam",
                new SourceValueCommandParameter(new(1, 5), new VariableReference("*list", new[] { "2" }))
            );

            // act
            var result = sut.Bind(command, commandInput);

            // assert
            Assert.True(result.Success);
            Assert.Equal("val1", command.NamedParameter1);
            Assert.Equal("val2", command.NamedParameter2);
        }

        [Fact]
        public void Bind_ExpandMapParameterUnknownNames_ErrorsIncludedInResult()
        {
            // arrange
            var map = new Map(new MapEntries
            {
                { "invalid", new Literal("something") },
                { "param1", new Literal("good") },
                { "invalid2", new Literal("something") }
            });
            var variables = new VariableCollection();
            variables.Set(new Variable("map", map));

            var sut = CreateCommandParameterBinder(variables, typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();

            var commandInput = CreateCommandInput(
                "nam",
                new SourceValueCommandParameter(new(1, 5), new VariableReference("*map"))
            );

            // act
            var result = sut.Bind(command, commandInput);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[] {
                new SourceError(new(1, 5), "Unknown named parameter 'invalid2'"),
                new SourceError(new(1, 5), "Unknown named parameter 'invalid'")
            }, result.Errors);
        }

        [Fact]
        public void Bind_ExpandMapParameterValueIsNotValid_ErrorsIncludedInResult()
        {
            // arrange
            var map = new Map(new MapEntries
            {
                { "param1", new ParameterNameCommandParameter(new(1, 1), "bad") }
            });
            var variables = new VariableCollection();
            variables.Set(new Variable("map", map));

            var replacer = new VariableReplacer();
            var sut = CreateCommandParameterBinder(variables, typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();

            var commandInput = CreateCommandInput(
                "nam",
                new SourceValueCommandParameter(new(1, 5), new VariableReference("*map"))
            );

            // act
            var result = sut.Bind(command, commandInput);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new SourceError(new (1, 5), "Value of map 'map' entry 'param1' is not a SourceValueCommandParameter."), result.Errors[0]);
        }

        [Fact]
        public void Bind_MapToDictionaryProperty_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(MapParameterCommand));
            var command = new MapParameterCommand();

            var map = new Map(new MapEntries
            {
                { "key1", new Literal("value1") },
                { "key2", new Literal("value2") }
            });

            var commandInput = CreateCommandInput(
                "map-params",
                new ParameterNameCommandParameter(new(1, 1), "dictionary"),
                map
            );

            // act
            var result = sut.Bind(command, commandInput);

            // assert
            Assert.True(result.Success);
            Assert.Equal(2, command.Dictionary.Count);
            Assert.Equal("value1", command.Dictionary["key1"]);
            Assert.Equal("value2", command.Dictionary["key2"]);
        }

        [Fact]
        public void Bind_MapToAbstractDictionaryProperty_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(MapParameterCommand));
            var command = new MapParameterCommand();

            var map = new Map(new MapEntries
            {
                { "key1", new Literal("value1") },
                { "key2", new Literal("value2") }
            });

            var commandInput = CreateCommandInput(
                "map-params",
                new ParameterNameCommandParameter(new(1, 1), "abstract-dictionary"),
                map
            );

            // act
            var result = sut.Bind(command, commandInput);

            // assert
            Assert.True(result.Success);
            Assert.Equal(2, command.AbstractDictionary.Count);
            Assert.Equal("value1", command.AbstractDictionary["key1"]);
            Assert.Equal("value2", command.AbstractDictionary["key2"]);
        }

        [Fact]
        public void Bind_MapToIntValueDictionaryProperty_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(MapParameterCommand));
            var command = new MapParameterCommand();

            var map = new Map(new MapEntries
            {
                { "key1", new Literal("42") },
                { "key2", new Literal("43") }
            });

            var commandInput = CreateCommandInput(
                "map-params",
                new ParameterNameCommandParameter(new(1, 1), "intdictionary"),
                map
            );

            // act
            var result = sut.Bind(command, commandInput);

            // assert
            Assert.True(result.Success);
            Assert.Equal(2, command.IntDictionary.Count);
            Assert.Equal(42, command.IntDictionary["key1"]);
            Assert.Equal(43, command.IntDictionary["key2"]);
        }

        [Fact]
        public void Bind_MapToListValueDictionaryProperty_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(MapParameterCommand));
            var command = new MapParameterCommand();

            var values1 = new[]{ new Literal("val1.1"), new Literal("val1.2") };
            var values2 = new[]{ new Literal("val2.1"), new Literal("val2.2") };

            var map = new Map(new MapEntries
            {
                { "key1", new Chel.Abstractions.Types.List(values1) },
                { "key2", new Chel.Abstractions.Types.List(values2) }
            });

            var commandInput = CreateCommandInput(
                "map-params",
                new ParameterNameCommandParameter(new(1, 1), "list-dictionary"),
                map
            );

            // act
            var result = sut.Bind(command, commandInput);

            // assert
            Assert.True(result.Success);
            Assert.Equal(2, command.ListDictionary.Count);
            Assert.Equal(values1, command.ListDictionary["key1"].Values);
            Assert.Equal(values2, command.ListDictionary["key2"].Values);
        }

        [Fact]
        public void Bind_MapKeyIncorrectType_ErrorsIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(MapParameterCommand));
            var command = new MapParameterCommand();

            var map = new Map(new MapEntries
            {
                { "key1", new Literal("value1") }
            });

            var commandInput = CreateCommandInput(
                "map-params",
                new ParameterNameCommandParameter(new(1, 1), "invalidkeytype"),
                new SourceValueCommandParameter(new (2, 3), map)
            );

            // act
            var result = sut.Bind(command, commandInput);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{
                new SourceError(new(2, 3), "Key type of property 'Chel.Abstractions.UnitTests.SampleCommands.MapParameterCommand.InvalidKeyTypeParam' must be 'string'.")
            }, result.Errors);
        }

        [Fact]
        public void Bind_SetLiteralOnMap_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(MapParameterCommand));
            var command = new MapParameterCommand();
            var input = CreateCommandInput(
                "map-params",
                new ParameterNameCommandParameter(new(1, 1), "dictionary"),
                new SourceValueCommandParameter(new(2, 3), new Literal("a"))
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new SourceError(new(2, 3), "Cannot bind a non-map to a map parameter 'dictionary'"), result.Errors[0]);
        }

        [Fact]
        public void Bind_SetMapOnLiteralProperty_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();

            var map = new Map(new MapEntries
            {
                { "key1", new Literal("value1") }
            });

            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter(new(1, 1), "string"),
                new SourceValueCommandParameter(new(2, 8), map)
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ new SourceError(new(2, 8), "Cannot bind a map to a non-map parameter 'string'") }, result.Errors);
        }

        private CommandParameterBinder CreateCommandParameterBinder(params Type[] commandTypes)
        {
            var variables = new VariableCollection();
            return CreateCommandParameterBinder(variables, commandTypes);
        }

        private CommandParameterBinder CreateCommandParameterBinder(VariableCollection variables, params Type[] commandTypes)
        {
            var registry = CreateCommandRegistry(commandTypes);
            var replacer = new VariableReplacer();

            return new CommandParameterBinder(registry, replacer, variables);
        }

        private CommandRegistry CreateCommandRegistry(params Type[] commandTypes)
        {
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);

            foreach(var commandType in commandTypes)
                registry.Register(commandType);

            return registry;
        }

        private CommandInput CreateCommandInput(string commandName, params ICommandParameter[] parameters)
        {
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, commandName);

            foreach(var parameter in parameters)
                builder.AddParameter(parameter);
            
            return builder.Build();
        }
    }
}