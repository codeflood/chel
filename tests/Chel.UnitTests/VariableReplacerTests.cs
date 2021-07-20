using System;
using System.Collections.Generic;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Variables;
using Chel.Exceptions;
using Xunit;

namespace Chel.UnitTests
{
    public class VariableReplacerTests
    {
        [Fact]
        public void ReplaceVariables_VariablesIsNull_ThrowsException()
        {
            // arange
            var sut = new VariableReplacer();
            Action sutAction = () => sut.ReplaceVariables(null, new LiteralCommandParameter("input"));

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variables", ex.ParamName);
        }

        [Fact]
        public void ReplaceVariables_InputIsNull_ThrowsException()
        {
            // arange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            Action sutAction = () => sut.ReplaceVariables(variables, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("input", ex.ParamName);
        }

        [Fact]
        public void ReplaceVariables_InputIsEmpty_ReturnsEmpty()
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();

            // act
            var result = sut.ReplaceVariables(variables, new AggregateCommandParameter(new LiteralCommandParameter[0]));

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public void ReplaceVariables_NoVariablesInInput_ReturnsInput()
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            var input = "Input without variables";

            // act
            var result = sut.ReplaceVariables(variables, new LiteralCommandParameter(input));

            // assert
            Assert.Equal(input, result);
        }

        [Theory]
        [MemberData(nameof(ReplaceVariables_InputContainsSetVariable_ReplacesVariable_DataSource))]
        public void ReplaceVariables_InputContainsSetVariable_ReplacesVariable(CommandParameter input, string expected)
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            variables.Set(new ValueVariable("foo", "bar"));

            // act
            var result = sut.ReplaceVariables(variables, input);

            // assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> ReplaceVariables_InputContainsSetVariable_ReplacesVariable_DataSource()
        {
            yield return new object[] {
                new AggregateCommandParameter(new CommandParameter[]{
                    new VariableCommandParameter("foo"),
                    new LiteralCommandParameter(" ipsum")
                }),
                "bar ipsum"
            };

            yield return new object[] {
                new AggregateCommandParameter(new CommandParameter[]{
                    new VariableCommandParameter("Foo"),
                    new LiteralCommandParameter(" ipsum")
                }),
                "bar ipsum"
            };

            yield return new object[] {
                new AggregateCommandParameter(new CommandParameter[]{
                    new LiteralCommandParameter("lorem "),
                    new VariableCommandParameter("FOO"),
                    new LiteralCommandParameter(" ipsum")
                }),
                "lorem bar ipsum"
            };

            yield return new object[] {
                new AggregateCommandParameter(new CommandParameter[]{
                    new LiteralCommandParameter("lorem "),
                    new VariableCommandParameter("foo")
                }),
                "lorem bar"
            };

            yield return new object[] {
                new AggregateCommandParameter(new CommandParameter[]{
                    new LiteralCommandParameter("lorem"),
                    new VariableCommandParameter("foo"),
                    new LiteralCommandParameter("ipsum")
                }),
                "lorembaripsum"
            };
        }

        [Fact]
        public void ReplaceVariables_VariableNotSet_ThrowsException()
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            Action sutAction = () => sut.ReplaceVariables(variables, new VariableCommandParameter("foo"));

            // act, assert
            var ex = Assert.Throws<UnsetVariableException>(sutAction);
            Assert.Equal("foo", ex.VariableName);
        }
    }
}
