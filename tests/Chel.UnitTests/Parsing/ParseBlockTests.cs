using System;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
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
        public void Ctor_BlockIsEmpty_DoesNotThrow()
        {
            var sut = new ParseBlock(new SourceLocation(1, 1), new CompoundValue(new Literal[0]));
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var location = new SourceLocation(1, 1);
            var parameter = new Literal("param");
            var sut = new ParseBlock(location, parameter);

            // assert
            Assert.Equal(location, sut.LocationStart);
            Assert.Equal(parameter, sut.Block);
        }

        [Fact]
        public void Ctor_EndOfLineIsTrue_SetsProperty()
        {
            // arrange, act
            var sut = new ParseBlock(new SourceLocation(1, 1), new Literal("param"), isEndOfLine: true);

            // assert
            Assert.True(sut.IsEndOfLine);
        }
    }
}