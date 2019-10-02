using Xunit;
using Chel.Abstractions.Results;
using System;

namespace Chel.Abstractions.UnitTests.Results
{
    public class ValueResultTests
    {
        [Fact]
        public void Ctor_ValueIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new ValueResult(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsValueAsString()
        {
            // arrange
            var sut = new ValueResult("the result");

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("the result", result);
        }
    }
}