using System;
using Chel.Exceptions;
using Xunit;

namespace Chel.UnitTests.Exceptions
{
    public class UnsetVariableExceptionTests
    {
        [Fact]
        public void Ctor_VariableNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new UnsetVariableException(null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variableName", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\n")]
        public void Ctor_VariableNameIsInvalid_ThrowsException(string variableName)
        {
            // arrange
            Action sutAction = () => new UnsetVariableException(variableName);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("variableName", ex.ParamName);
        }

        [Fact]
        public void VariableName_WhenCalled_ReturnsVariableName()
        {
            // arrange
            var sut = new UnsetVariableException("var");

            // act
            var result = sut.VariableName;

            // assert
            Assert.Equal("var", result);
        }
    }
}