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
            Action sutAction = () => new NumberedParameterAttribute(number);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("number", ex.ParamName);
            Assert.Contains("number must be greater than 0", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new NumberedParameterAttribute(4);

            // assert
            Assert.Equal(4, sut.Number);
        }
    }
}