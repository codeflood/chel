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

        [Fact]
        public void Ctor_MessageIsEmpty_ThrowsException()
        {
            // arrange
            var action = () => new SourceError(new SourceLocation(1, 1), "");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("message", ex.ParamName);
            Assert.Contains("'error' cannot be empty", ex.Message);
        }
    }
}