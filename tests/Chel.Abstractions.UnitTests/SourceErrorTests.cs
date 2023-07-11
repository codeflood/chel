using System;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class SourceErrorTests
    {
        [Fact]
        public void Ctor_MessageIsNull_ThrowsException()
        {
            // arrange
            var action = () => new SourceError(new SourceLocation(1, 1), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("message", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\t")]
        public void Ctor_MessageIsEmpty_ThrowsException(string message)
        {
            // arrange
            var action = () => new SourceError(new SourceLocation(1, 1), message);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("message", ex.ParamName);
            Assert.Contains("'message' cannot be empty or whitespace", ex.Message);
        }
    }
}