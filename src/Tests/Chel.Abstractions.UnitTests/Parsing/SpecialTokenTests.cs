using System;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class SpecialTokenTests
    {
        [Fact]
        public void Ctor_LocationIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new SpecialToken(null, SpecialTokenType.Undefined);

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
            var sut = new SpecialToken(location, SpecialTokenType.BlockStart);

            // assert
            Assert.Equal(2, sut.Location.LineNumber);
            Assert.Equal(3, sut.Location.CharacterNumber);
            Assert.Equal(SpecialTokenType.BlockStart, sut.Type);
        }
    }
}