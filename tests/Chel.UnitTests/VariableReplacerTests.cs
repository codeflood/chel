using System;
using System.Collections.Generic;
using Chel.Abstractions.Types;
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
            Action sutAction = () => sut.ReplaceVariables(null, new Literal("input"));

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
            var result = sut.ReplaceVariables(variables, new SingleValue(new Literal[0]));

            // assert
            var singleValueResult = Assert.IsType<SingleValue>(result);
            Assert.Empty(singleValueResult.Values);
        }

        [Fact]
        public void ReplaceVariables_NoVariablesInInput_ReturnsInput()
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            var input = "Input without variables";

            // act
            var result = sut.ReplaceVariables(variables, new Literal(input));

            // assert
            var literalResult = Assert.IsType<Literal>(result);
            Assert.Equal(input, literalResult.Value);
        }

        [Theory]
        [MemberData(nameof(ReplaceVariables_InputContainsSetVariable_ReplacesVariable_DataSource))]
        public void ReplaceVariables_InputContainsSetVariable_ReplacesVariable(ChelType input, string expected)
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            variables.Set(new Variable("foo", new Literal("bar")));

            // act
            var result = sut.ReplaceVariables(variables, input);

            // assert
            var singleValueResult = Assert.IsType<SingleValue>(result);
            var resultValue = string.Join("", singleValueResult.Values);
            Assert.Equal(expected, resultValue);
        }

        public static IEnumerable<object[]> ReplaceVariables_InputContainsSetVariable_ReplacesVariable_DataSource()
        {
            yield return new object[] {
                new SingleValue(new ChelType[]{
                    new VariableReference("foo"),
                    new Literal(" ipsum")
                }),
                "bar ipsum"
            };

            yield return new object[] {
                new SingleValue(new ChelType[]{
                    new VariableReference("Foo"),
                    new Literal(" ipsum")
                }),
                "bar ipsum"
            };

            yield return new object[] {
                new SingleValue(new ChelType[]{
                    new Literal("lorem "),
                    new VariableReference("FOO"),
                    new Literal(" ipsum")
                }),
                "lorem bar ipsum"
            };

            yield return new object[] {
                new SingleValue(new ChelType[]{
                    new Literal("lorem "),
                    new VariableReference("foo")
                }),
                "lorem bar"
            };

            yield return new object[] {
                new SingleValue(new ChelType[]{
                    new Literal("lorem"),
                    new VariableReference("foo"),
                    new Literal("ipsum")
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
            Action sutAction = () => sut.ReplaceVariables(variables, new VariableReference("foo"));

            // act, assert
            var ex = Assert.Throws<UnsetVariableException>(sutAction);
            Assert.Equal("foo", ex.VariableName);
        }

        [Fact]
        public void ReplaceVariables_InputIsListWithNoVariables_ReturnsListUnaltered()
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            var input = new List(new[]{ new Literal("val1") });

            // act
            var result = sut.ReplaceVariables(variables, input);

            // assert
            var listResult = Assert.IsType<List>(result);
            Assert.Equal(input, listResult);
        }

        [Fact]
        public void ReplaceVariables_InputIsListWithVariables_ReturnsListWithVariablesReplaced()
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            variables.Set(new Variable("foo1", new Literal("bar1")));
            variables.Set(new Variable("foo2", new Literal("bar2")));

            var input = new List(new ChelType[]
            {
                new VariableReference("foo1"),
                new Literal("val"),
                new VariableReference("foo2")
            });

            // act
            var result = sut.ReplaceVariables(variables, input);

            // assert
            var listResult = Assert.IsType<List>(result);
            Assert.Equal(new List(new[] {
                new Literal("bar1"),
                new Literal("val"),
                new Literal("bar2")
            }), listResult);
        }
    }
}
