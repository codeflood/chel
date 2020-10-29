using System;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
    public class LiteralTokenTests
    {
        [Fact]
        public void Ctor_VariableNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new LiteralToken(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("value", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("value with space\nand newline")]
        [InlineData("value")]
        [InlineData("💾")]
        public void Ctor_WhenCalled_SetsProperties(string value)
        {
            // arrange, act
            var sut = new LiteralToken(value);

            // assert
            Assert.Equal(value, sut.Value);
        }
    }
}
