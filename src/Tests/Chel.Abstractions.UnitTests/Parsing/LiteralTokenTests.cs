using System;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class LiteralTokenTests
    {
        [Fact]
        public void Ctor_LocationIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new LiteralToken(null, 'a');

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("location", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var location = new SourceLocation(2, 3);

            // act
            var sut = new LiteralToken(location, 'b');

            // assert
            Assert.Equal(2, sut.Location.LineNumber);
            Assert.Equal(3, sut.Location.CharacterNumber);
            Assert.Equal('b', sut.Value);
        }
    }
}