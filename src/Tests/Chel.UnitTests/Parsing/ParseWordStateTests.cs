using System.Linq;
using Chel.Abstractions.Parsing;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
    public class ParseWordStateTests
    {
        [Fact]
        public void Process_InputIsNonSpecialCharacter_ReturnsContinue()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            var result = sut.Process('a');

            // assert
            Assert.IsType<ContinueResponse>(result.Single());
        }

        [Fact]
        public void Process_InputIsWhiteSpace_ReturnsSetStateToWhiteSpace()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            var result = sut.Process(' ');

            // assert
            var setStateRespones = result.ElementAt(0) as SetStateResponse;
            Assert.NotNull(setStateRespones);
            Assert.IsType<SkipWhiteSpaceState>(setStateRespones.State);
        }

        [Fact]
        public void Process_PreviousCharactersAndInputIsWhiteSpace_ReturnsEmitAndSetStateToWhiteSpace()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process('a');
            sut.Process('b');
            sut.Process('c');
            var result = sut.Process(' ');

            // assert
            var emitResponse = result.ElementAt(0) as EmitResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("abc", (emitResponse.Token as LiteralToken).Value);

            var setStateRespones = result.ElementAt(1) as SetStateResponse;
            Assert.NotNull(setStateRespones);
            Assert.IsType<SkipWhiteSpaceState>(setStateRespones.State);
        }

        [Fact]
        public void Process_InputIsCommentCharacter_ReturnsSetStateToComment()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            var result = sut.Process(Symbols.Comment);

            // assert
            var setStateRespones = result.ElementAt(0) as SetStateResponse;
            Assert.NotNull(setStateRespones);
            Assert.IsType<SkipCommentState>(setStateRespones.State);
        }

        [Fact]
        public void Process_InputIsCommentCharacter_ReturnsEmitAndSetStateToComment()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process('h');
            var result = sut.Process(Symbols.Comment);

            // assert
            var emitResponse = result.ElementAt(0) as EmitResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("h", (emitResponse.Token as LiteralToken).Value);

            var setStateRespones = result.ElementAt(1) as SetStateResponse;
            Assert.NotNull(setStateRespones);
            Assert.IsType<SkipCommentState>(setStateRespones.State);
        }

        [Fact]
        public void Process_EscapedComment_EmitsTokenWithCommentSymbol()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process('a');
            sut.Process(Symbols.Escape);
            sut.Process(Symbols.Comment);
            var result = sut.Process(' ');

            // assert
            var emitResponse = result.ElementAt(0) as EmitResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("a#", (emitResponse.Token as LiteralToken).Value);
        }

        [Fact]
        public void Process_EscapedEscapedSymbol_EmitsEscapeSymbol()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.Escape);
            sut.Process(Symbols.Escape);
            var result = sut.Process('\n');

            // assert
            var emitResponse = result.ElementAt(0) as EmitResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("\\", (emitResponse.Token as LiteralToken).Value);
        }
    }
}