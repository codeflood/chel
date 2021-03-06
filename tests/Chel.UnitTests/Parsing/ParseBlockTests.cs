using System;
using Chel.Abstractions.Parsing;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
    public class ParseBlockTests
    {
        [Fact]
        public void Ctor_BlockIsNull_DoesNotThrow()
        {
            new ParseBlock(new SourceLocation(1, 1), null);
        }

        [Fact]
        public void Ctor_LocationIsNull_Throws()
        {
            // arrange
            Action sutAction = () => new ParseBlock(null, "abc");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("locationStart", ex.ParamName);
        }

        [Fact]
        public void Ctor_BlockIsEmpty_DoesNotThrow()
        {
            var sut = new ParseBlock(new SourceLocation(1, 1), "");
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var location = new SourceLocation(1, 1);
            var sut = new ParseBlock(location, "param");

            // assert
            Assert.Equal(location, sut.LocationStart);
            Assert.Equal("param", sut.Block);
        }

        [Fact]
        public void Ctor_EndOfLineIsTrue_SetsProperty()
        {
            // arrange, act
            var sut = new ParseBlock(new SourceLocation(1, 1), "param", isEndOfLine: true);

            // assert
            Assert.True(sut.IsEndOfLine);
        }
    }
}