using System;
using Xunit;

namespace Chel.UnitTests
{
    public class ParseBlockTests
    {
        [Fact]
        public void Ctor_BlockIsNull_DoesNotThrow()
        {
            Action sutAction = () => new ParseBlock(null);
        }

        [Fact]
        public void Ctor_BlockIsEmpty_DoesNotThrow()
        {
            var sut = new ParseBlock("");
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperty()
        {
            // arrange, act
            var sut = new ParseBlock("param");

            // assert
            Assert.Equal("param", sut.Block);
        }

        [Fact]
        public void Ctor_EndOfLineIsTrue_SetsProperty()
        {
            // arrange, act
            var sut = new ParseBlock("param", isEndOfLine: true);

            // assert
            Assert.True(sut.IsEndOfLine);
        }

        [Fact]
        public void Ctor_NameIsTrue_SetsProperty()
        {
            // arrange, act
            var sut = new ParseBlock("param", isName: true);

            // assert
            Assert.True(sut.IsName);
        }
    }
}