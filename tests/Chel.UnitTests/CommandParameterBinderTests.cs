using System;
using System.Collections.Generic;
using Chel.Abstractions.Parsing;
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
            var input = CreateCommandInput("num", new LiteralCommandParameter("value"));

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
            var input = CreateCommandInput("num", new LiteralCommandParameter("value1"), new LiteralCommandParameter("value2"));

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
                new LiteralCommandParameter("value1"),
                new LiteralCommandParameter("value2"),
                new LiteralCommandParameter("value3")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unexpected numbered parameter 'value3'" }, result.Errors);
        }

        [Fact]
        public void Bind_TwoTooManyNumberedParametersProvided_ErrorsIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput(
                "num",
                new LiteralCommandParameter("value1"), 
                new LiteralCommandParameter("value2"),
                new LiteralCommandParameter("value3"),
                new LiteralCommandParameter("value4")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unexpected numbered parameter 'value3'", "Unexpected numbered parameter 'value4'" }, result.Errors);
        }

        [Fact]
        public void Bind_NumberedParameterStartsWithEscapedDash_PropertyIsSet()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NumberedParameterCommand));
            var command = new NumberedParameterCommand();
            var input = CreateCommandInput("num", new LiteralCommandParameter("-value"));

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
            var input = CreateCommandInput("num", new LiteralCommandParameter("val-ue"));

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
            var input = CreateCommandInput("command", new LiteralCommandParameter("3000000000"));

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Contains("Invalid parameter value '3000000000' for numbered parameter 'num'.", result.Errors);
        }

        [Fact]
        public void Bind_NamedParameterOnCommand_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter("param1"),
                new LiteralCommandParameter("value1")
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
                new ParameterNameCommandParameter("PARAM1"),
                new LiteralCommandParameter("value1")
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
            var input = CreateCommandInput("nam", new ParameterNameCommandParameter("param1"));
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Missing value for named parameter 'param1'" }, result.Errors);
        }

        [Fact]
        public void Bind_NamedParameterValueStartsWithDash_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", new ParameterNameCommandParameter("param1"), new LiteralCommandParameter("-value1"));
            
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
                new ParameterNameCommandParameter("param1"),
                new LiteralCommandParameter("value1"),
                new ParameterNameCommandParameter("param2"),
                new LiteralCommandParameter("value2")
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
                new ParameterNameCommandParameter("ParaM1"),
                (LiteralCommandParameter)"value1",
                new ParameterNameCommandParameter("ParaM2"),
                (LiteralCommandParameter)"value2"
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
                new ParameterNameCommandParameter("param1"),
                (LiteralCommandParameter)"value1",
                new ParameterNameCommandParameter("param1"),
                (LiteralCommandParameter)"value2"
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat named parameter 'param1'" }, result.Errors);
        }

        [Fact]
        public void Bind_DoubleDuplicateNamedParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter("param1"),
                (LiteralCommandParameter)"value1",
                new ParameterNameCommandParameter("param1"),
                (LiteralCommandParameter)"value2",
                new ParameterNameCommandParameter("param1"),
                (LiteralCommandParameter)"value3"
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat named parameter 'param1'" }, result.Errors);
        }

        [Fact]
        public void Bind_TripleDuplicateNamedParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter("param1"),
                (LiteralCommandParameter)"value1",
                new ParameterNameCommandParameter("param1"),
                (LiteralCommandParameter)"value2",
                new ParameterNameCommandParameter("param1"),
                (LiteralCommandParameter)"value3",
                new ParameterNameCommandParameter("param1"),
                (LiteralCommandParameter)"value4"
            );
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat named parameter 'param1'" }, result.Errors);
        }

        [Fact]
        public void Bind_UnknownNamedParameter_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(NamedParameterCommand));
            var command = new NamedParameterCommand();
            var input = CreateCommandInput(
                "nam",
                new ParameterNameCommandParameter("invalid"),
                (LiteralCommandParameter)"value"
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unknown named parameter 'invalid'" }, result.Errors);
        }

        [Fact]
        public void Bind_FlagParameter_BindsProperty()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter("p1"),
                new ParameterNameCommandParameter("p2")
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
                new ParameterNameCommandParameter("unknown1"),
                new ParameterNameCommandParameter("unknown2")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Unknown flag parameter 'unknown2'", "Unknown flag parameter 'unknown1'" }, result.Errors);
        }

        [Fact]
        public void Bind_DuplicateFlagParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter("p1"),
                new ParameterNameCommandParameter("p1")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat flag parameter 'p1'" }, result.Errors);
        }

        [Fact]
        public void Bind_DoubleDuplicateFlagParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter("p1"),
                new ParameterNameCommandParameter("p1"),
                new ParameterNameCommandParameter("p1")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat flag parameter 'p1'" }, result.Errors);
        }

        [Fact]
        public void Bind_TripleDuplicateFlagParameterOnCommand_ErrorIncludedInResult()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(FlagParameterCommand));
            var command = new FlagParameterCommand();
            var input = CreateCommandInput(
                "command",
                new ParameterNameCommandParameter("p1"),
                new ParameterNameCommandParameter("p1"),
                new ParameterNameCommandParameter("p1"),
                new ParameterNameCommandParameter("p1")
            );

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Cannot repeat flag parameter 'p1'" }, result.Errors);
        }

        [Fact]
        public void Bind_NoSetterOnProperty_ThrowsException()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(ParameterNoSetterCommand));
            var command = new ParameterNoSetterCommand();
            var input = CreateCommandInput("command", (LiteralCommandParameter)"parameter");

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
            Assert.Equal(new[]{ "Missing required numbered parameter 'param'" }, result.Errors);
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
            Assert.Equal(new[]{ "Missing required named parameter 'param'" }, result.Errors);
        }

        [Fact]
        public void Bind_RequiredParameterProvided_PropertyIsSet()
        {
            // arrange
            var sut = CreateCommandParameterBinder(typeof(RequiredParameterCommand));
            var command = new RequiredParameterCommand();
            var input = CreateCommandInput("command", (LiteralCommandParameter)"val");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal("val", command.NumberedParameter);
        }

        [Theory]
        [MemberData(nameof(Bind_NumberedAndNamedParameters_BindsProperty_Datasource))]
        public void Bind_NumberedAndNamedParameters_BindsProperty(CommandParameter parameter1, CommandParameter parameter2, CommandParameter parameter3)
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
                new LiteralCommandParameter("num1"), 
                new ParameterNameCommandParameter("named"),
                new LiteralCommandParameter("value1")
            };

            yield return new object[] {
                new ParameterNameCommandParameter("named"),
                new LiteralCommandParameter("value1"),
                new LiteralCommandParameter("num1"),
            };
        }

        [Theory]
        [MemberData(nameof(Bind_NumberedAndFlagParameters_BindsProperty_Datasource))]
        public void Bind_NumberedAndFlagParameters_BindsProperty(CommandParameter parameter1, CommandParameter parameter2)
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
                new LiteralCommandParameter("num1"), 
                new ParameterNameCommandParameter("flag")
            };

            yield return new object[] {
                new ParameterNameCommandParameter("flag"),
                new LiteralCommandParameter("num1"),
            };
        }

        [Theory]
        [MemberData(nameof(Bind_NamedAndFlagParameters_BindsProperty_Datasource))]
        public void Bind_NamedAndFlagParameters_BindsProperty(CommandParameter parameter1, CommandParameter parameter2, CommandParameter parameter3)
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
                new ParameterNameCommandParameter("named"),
                new LiteralCommandParameter("value1"), 
                new ParameterNameCommandParameter("flag")
            };

            yield return new object[] {
                new ParameterNameCommandParameter("flag"),
                new ParameterNameCommandParameter("named"),
                new LiteralCommandParameter("value1")
            };
        }

        [Theory]
        [MemberData(nameof(Bind_AllParameters_BindsProperty_Datasource))]
        public void Bind_AllParameters_BindsProperty(
            CommandParameter parameter1,
            CommandParameter parameter2,
            CommandParameter parameter3,
            CommandParameter parameter4)
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
                new ParameterNameCommandParameter("flag"),
                new ParameterNameCommandParameter("named"),
                new LiteralCommandParameter("value1"), 
                new LiteralCommandParameter("num1")
            };

            yield return new object[] {
                new ParameterNameCommandParameter("flag"),
                new LiteralCommandParameter("num1"),
                new ParameterNameCommandParameter("named"),
                new LiteralCommandParameter("value1")
            };

            yield return new object[] {
                new ParameterNameCommandParameter("named"),
                new LiteralCommandParameter("value1"),
                new ParameterNameCommandParameter("flag"),
                new LiteralCommandParameter("num1")
            };

            yield return new object[] {
                new ParameterNameCommandParameter("named"),
                new LiteralCommandParameter("value1"),
                new LiteralCommandParameter("num1"),
                new ParameterNameCommandParameter("flag")
            };

            yield return new object[] {
                new LiteralCommandParameter("num1"),
                new ParameterNameCommandParameter("named"),
                new LiteralCommandParameter("value1"),
                new ParameterNameCommandParameter("flag")
            };

            yield return new object[] {
                new LiteralCommandParameter("num1"),
                new ParameterNameCommandParameter("flag"),
                new ParameterNameCommandParameter("named"),
                new LiteralCommandParameter("value1")
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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("bool"), (LiteralCommandParameter)value);

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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("byte"), (LiteralCommandParameter)value);

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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("enum"), (LiteralCommandParameter)value);

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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("int"), (LiteralCommandParameter)value);

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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("char"), (LiteralCommandParameter)value);

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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("float"), (LiteralCommandParameter)value);

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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("double"), (LiteralCommandParameter)value);

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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("date"), (LiteralCommandParameter)value);

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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("time"), (LiteralCommandParameter)value);

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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("guid"), (LiteralCommandParameter)value);

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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("guid"), (LiteralCommandParameter)value);

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Contains($"Invalid parameter value '{value}' for named parameter 'guid'.", result.Errors);
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
            var input = CreateCommandInput("command", new ParameterNameCommandParameter(parameter), (LiteralCommandParameter)value);

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Contains($"Invalid parameter value '{value}' for named parameter '{parameter}'.", result.Errors);
        }

        [Fact]
        public void Bind_PropertyHasTypeConverter_BindsParameter()
        {
            // arrange
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var sut = CreateCommandParameterBinder(typeof(ParameterTypesCommand));
            var command = new ParameterTypesCommand();
            var input = CreateCommandInput("command", new ParameterNameCommandParameter("guidarray"), (LiteralCommandParameter)$"{guid1}|{guid2}");

            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.True(result.Success);
            Assert.Equal(command.GuidArray, new[]{ guid1, guid2 });
        }

        [Fact]
        public void Bind_NamedParameterContainsVariable_VariableIsReplaced()
        {
            // arrange
            var registry = CreateCommandRegistry(typeof(NamedParameterCommand));
            var variables = new VariableCollection();
            variables.Set(new ValueVariable("foo", "bar"));

            var replacer = new VariableReplacer();

            var sut = new CommandParameterBinder(registry, replacer, variables);
            var command = new NamedParameterCommand();
            var input = CreateCommandInput("nam", new ParameterNameCommandParameter("param1"), new VariableCommandParameter("foo"));
            
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
            var input = CreateCommandInput("nam", new ParameterNameCommandParameter("param1"), new VariableCommandParameter("foo"));
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Variable $foo$ is not set" }, result.Errors);
        }

        [Fact]
        public void Bind_NumberedParameterContainsVariable_VariableIsReplaced()
        {
            // arrange
            var registry = CreateCommandRegistry(typeof(NumericNumberedParameterCommand));
            var variables = new VariableCollection();
            variables.Set(new ValueVariable("foo", "10"));

            var replacer = new VariableReplacer();

            var sut = new CommandParameterBinder(registry, replacer, variables);
            var command = new NumericNumberedParameterCommand();
            var input = CreateCommandInput("command", new VariableCommandParameter("foo"));
            
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
            var input = CreateCommandInput("command", new VariableCommandParameter("foo"));
            
            // act
            var result = sut.Bind(command, input);

            // assert
            Assert.False(result.Success);
            Assert.Equal(new[]{ "Variable $foo$ is not set" }, result.Errors);
        }

        private CommandParameterBinder CreateCommandParameterBinder(params Type[] commandTypes)
        {
            var registry = CreateCommandRegistry(commandTypes);
            var variables = new VariableCollection();
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

        private CommandInput CreateCommandInput(string commandName, params CommandParameter[] parameters)
        {
            var builder = new CommandInput.Builder(1, commandName);

            foreach(var parameter in parameters)
                builder.AddParameter(parameter);
            
            return builder.Build();
        }
    }
}