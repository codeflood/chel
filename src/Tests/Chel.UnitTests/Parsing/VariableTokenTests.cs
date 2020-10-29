using System;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
    public class VariableTokenTests
    {
        [Fact]
        public void Ctor_VariableNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new VariableToken(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variableName", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void Ctor_VariableNameIsEmptyOrWhitespace_ThrowsException(string variableName)
        {
            // arrange
            Action sutAction = () => new VariableToken(variableName);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("variableName", ex.ParamName);
        }

        [Theory]
        [InlineData("var")]
        [InlineData("💾")]
        public void Ctor_WhenCalled_SetsProperties(string variableName)
        {
            // arrange, act
            var sut = new VariableToken(variableName);

            // assert
            Assert.Equal(variableName, sut.VariableName);
        }
    }
}