using System.Linq;
using Chel.Abstractions.Parsing;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
    public class ParseLiteralInnerStateTests
    {
        [Fact]
        public void Process_InputIsNotEscapeSymbol_ReturnsEmptyEmit()
        {
            // arrange
            var sut = new ParseLiteralInnerState();

            // act
            var result = sut.Process('a');

            // assert
            var emitResponse = Assert.IsType<EmitResponse>(result.First());
            var token = Assert.IsType<LiteralToken>(emitResponse.Token);
            Assert.Empty(token.Value);
        }

        [Fact]
        public void Process_InputIsEscapeSymbol_ReturnsContinue()
        {
            // arrange
            var sut = new ParseLiteralInnerState();

            // act
            var result = sut.Process(Symbols.Escape);

            // assert
            Assert.IsType<ContinueResponse>(result.First());
        }

        [Theory]
        [InlineData('a')]
        [InlineData(Symbols.BlockEnd)]
        [InlineData(Symbols.BlockStart)]
        [InlineData(Symbols.Comment)]
        [InlineData(Symbols.Escape)]
        [InlineData(Symbols.ParameterName)]
        [InlineData(Symbols.Variable)]
        public void Process_EscapedCharacter_ReturnsEmit(char secondChar)
        {
            // arrange
            var sut = new ParseLiteralInnerState();

            // act
            sut.Process(Symbols.Escape);
            var result = sut.Process(secondChar);

            // assert
            var emitResponse = Assert.IsType<EmitResponse>(result.First());
            var token = Assert.IsType<LiteralToken>(emitResponse.Token);
            Assert.Equal(secondChar.ToString(), token.Value);
        }

        [Theory]
        [InlineData(' ')]
        [InlineData('\n')]
        [InlineData('\t')]
        [InlineData('\r')]
        public void Process_InputIsEscapedSpace_ReturnsEmptyEmit(char secondChar)
        {
            // arrange
            var sut = new ParseLiteralInnerState();

            // act
            sut.Process(Symbols.Escape);
            var result = sut.Process(secondChar);

            // assert
            var emitResponse = Assert.IsType<EmitResponse>(result.First());
            var token = Assert.IsType<LiteralToken>(emitResponse.Token);
            Assert.Empty(token.Value);
        }
    }
}
