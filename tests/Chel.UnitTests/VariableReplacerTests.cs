using System;
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
            Action sutAction = () => sut.ReplaceVariables(null, "input");

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
            var result = sut.ReplaceVariables(variables, "");

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
            var result = sut.ReplaceVariables(variables, input);

            // assert
            Assert.Equal(input, result);
        }

        [Theory]
        [InlineData("$foo$ ipsum", "bar ipsum")]
        [InlineData("$Foo$ ipsum", "bar ipsum")]
        [InlineData("lorem $FOO$ ipsum", "lorem bar ipsum")]
        [InlineData("lorem $foo$", "lorem bar")]
        [InlineData("lorem$foo$ipsum", "lorembaripsum")]
        public void ReplaceVariables_InputContainsSetVariable_ReplacesVariable(string input, string expected)
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

        [Theory]
        [InlineData("lorem foo$ ipsum")]
        [InlineData("lorem $foo ipsum")]
        [InlineData("$")]
        public void ReplaceVariables_MissingPairedVariableToken_ThrowsException(string input)
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            variables.Set(new ValueVariable("foo", "bar"));

            Action sutAction = () => sut.ReplaceVariables(variables, input);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("input", ex.ParamName);
            Assert.Contains("Unpaired variable token $", ex.Message);
        }

        [Fact]
        public void ReplaceVariables_VariableSymbolEscaped_ReturnsWithUnescapedVariable()
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            variables.Set(new ValueVariable("foo", "bar"));

            // act
            var result = sut.ReplaceVariables(variables, @"lorem \$foo\$ ipsum");

            // assert
            Assert.Equal(@"lorem $foo$ ipsum", result);
        }

        [Fact]
        public void ReplaceVariables_VariableNotSet_ThrowsException()
        {
            // arrange
            var sut = new VariableReplacer();
            var variables = new VariableCollection();
            Action sutAction = () => sut.ReplaceVariables(variables, "$foo$");

            // act, assert
            var ex = Assert.Throws<UnsetVariableException>(sutAction);
            Assert.Equal("foo", ex.VariableName);
        }
    }
}
