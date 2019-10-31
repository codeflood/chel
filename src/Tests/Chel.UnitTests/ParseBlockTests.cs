using System;
using Xunit;

namespace Chel.UnitTests
{
    public class ParseBlockTests
    {
        [Fact]
        public void Ctor_BlockIsNull_DoesNotThrow()
        {
            Action sutAction = () => new ParseBlock(null, false);
        }

        [Fact]
        public void Ctor_BlockIsEmpty_DoesNotThrow()
        {
            var sut = new ParseBlock("", false);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new ParseBlock("param", true);

            // assert
            Assert.Equal("param", sut.Block);
            Assert.True(sut.EndOfLine);
        }
    }
}