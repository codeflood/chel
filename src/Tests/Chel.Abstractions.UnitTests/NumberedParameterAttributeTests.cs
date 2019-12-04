using System;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class NumberedParameterAttributeTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Ctor_NumberIsZeroOrLess_ThrowsException(int number)
        {
            // arrange
            Action sutAction = () => new NumberedParameterAttribute(number, "param");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("number", ex.ParamName);
            Assert.Contains("number must be greater than 0", ex.Message);
        }

        [Fact]
        public void Ctor_PlaceholderTextIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new NumberedParameterAttribute(1, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("placeholderText", ex.ParamName);
        }

        [Fact]
        public void Ctor_PlaceholderTextIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new NumberedParameterAttribute(1, "");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("placeholderText", ex.ParamName);
            Assert.Contains("placeholderText cannot be empty", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new NumberedParameterAttribute(4, "param");

            // assert
            Assert.Equal(4, sut.Number);
            Assert.Equal("param", sut.PlaceholderText);
        }
    }
}