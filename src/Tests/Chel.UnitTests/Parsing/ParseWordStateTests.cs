using System.Linq;
using Chel.Abstractions.Parsing;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
    public class ParseWordStateTests
    {
        [Theory]
        [InlineData('a')]
        [InlineData('A')]
        [InlineData(Symbols.Variable)]
        [InlineData(Symbols.BlockStart)]
        [InlineData(Symbols.BlockEnd)]
        [InlineData(Symbols.Comment)]
        public void CanProcess_InputIsNotWhiteSpace_ReturnsTrue(char input)
        {
            // arrange
            var sut = new ParseWordState();

            // act
            var result = sut.CanProcess(input);

            // assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(' ')]
        [InlineData('\t')]
        [InlineData('\r')]
        public void CanProcess_InputIsWhiteSpace_ReturnsFalse(char input)
        {
            // arrange
            var sut = new ParseWordState();

            // act
            var result = sut.CanProcess(input);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Process_InputIsNonSpecialCharacter_ReturnsContinue()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            var result = sut.Process('a');

            // assert
            Assert.IsType<ContinueResponse>(result);
        }

        [Fact]
        public void Process_FistInputIsWhiteSpace_ReturnsStepDown()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            var result = sut.Process(' ');

            // assert
            Assert.IsType<StepDownResponse>(result);
        }

        // completed

        [Fact]
        public void Process_WordThenWhitespace_ReturnsEmitThenStepDown()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process('a');
            sut.Process('b');
            sut.Process('c');
            var result1 = sut.Process(' ');
            var result2 = sut.Process('a');

            // assert
            var emitResponse = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("abc", (emitResponse.Token as LiteralToken).Value);

            Assert.IsType<StepDownResponse>(result2);
        }

        [Fact]
        public void Process_WordWithAttachedComment_EmitsWord()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process('a');
            sut.Process('b');
            sut.Process(Symbols.Comment);
            sut.Process('c');
            var result1 = sut.Process(' ');

            // assert
            var emitResponse = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("ab", (emitResponse.Token as LiteralToken).Value);
        }

        [Fact]
        public void Process_ParenthesisedWordThenWhitespace_ReturnsEmitThenStepDown()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.BlockStart);
            sut.Process('a');
            sut.Process('b');
            sut.Process('c');
            var result1 = sut.Process(Symbols.BlockEnd);
            var result2 = sut.Process(' ');
            sut.Process('a');

            // assert
            var emitResponse = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("abc", (emitResponse.Token as LiteralToken).Value);

            Assert.IsType<StepDownResponse>(result2);
        }

        [Fact]
        public void Process_ParenthesisedWordWithSpacesThenWhitespace_ReturnsEmitThenStepDown()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.BlockStart);
            sut.Process('a');
            sut.Process(' ');
            sut.Process('b');
            sut.Process('c');
            var result1 = sut.Process(Symbols.BlockEnd);
            var result2 = sut.Process(' ');
            sut.Process('a');

            // assert
            var emitResponse = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("a bc", (emitResponse.Token as LiteralToken).Value);

            Assert.IsType<StepDownResponse>(result2);
        }

        [Fact]
        public void Process_ParenthesisedSpaceThenWhitespace_ReturnsEmitThenStepDown()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.BlockStart);
            sut.Process(' ');
            var result1 = sut.Process(Symbols.BlockEnd);
            var result2 = sut.Process('\n');
            sut.Process('a');

            // assert
            var emitResponse = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal(" ", (emitResponse.Token as LiteralToken).Value);

            Assert.IsType<StepDownResponse>(result2);
        }

        [Fact]
        public void Process_ParenthesisedCommentSymbolThenWhitespace_ReturnsEmitThenStepDown()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.BlockStart);
            sut.Process(Symbols.Comment);
            var result1 = sut.Process(Symbols.BlockEnd);
            var result2 = sut.Process(' ');
            sut.Process('a');

            // assert
            var emitResponse = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal(Symbols.Comment.ToString(), (emitResponse.Token as LiteralToken).Value);

            Assert.IsType<StepDownResponse>(result2);
        }

        [Fact]
        public void Process_ParenthesisedCommentSymbolWithLiteralThenWhitespace_ReturnsEmitThenStepDown()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.BlockStart);
            sut.Process('l');
            sut.Process('i');
            sut.Process('t');
            sut.Process(Symbols.Comment);
            sut.Process('a');
            var result1 = sut.Process(Symbols.BlockEnd);
            var result2 = sut.Process(' ');
            sut.Process('a');

            // assert
            var emitResponse = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("lit#a", (emitResponse.Token as LiteralToken).Value);

            Assert.IsType<StepDownResponse>(result2);
        }

        [Fact]
        public void Process_ParenthesisedCommentSymbolOnNewLineThenWhitespace_ReturnsEmitThenStepDown()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.BlockStart);
            sut.Process('l');
            sut.Process('i');
            sut.Process('t');
            sut.Process('\n');
            sut.Process(Symbols.Comment);
            sut.Process('a');
            var result1 = sut.Process(Symbols.BlockEnd);
            var result2 = sut.Process(' ');
            sut.Process('a');

            // assert
            var emitResponse = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("lit\n#a", (emitResponse.Token as LiteralToken).Value);

            Assert.IsType<StepDownResponse>(result2);
        }

        [Fact]
        public void Process_ParenthesisedCommentSymbolOnNewLineWithBracketThenWhitespace_ReturnsEmitThenStepDown()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.BlockStart);
            sut.Process('l');
            sut.Process('i');
            sut.Process('t');
            sut.Process('\n');
            sut.Process(Symbols.Comment);
            sut.Process('a');

            // This bracket is on the same line as the comment, so should be treated as part of the comment.
            sut.Process(Symbols.BlockEnd);
            sut.Process('\n');
            var result1 = sut.Process(Symbols.BlockEnd);
            var result2 = sut.Process(' ');
            sut.Process('a');

            // assert
            var emitResponse = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("lit\n#a)\n", (emitResponse.Token as LiteralToken).Value);

            Assert.IsType<StepDownResponse>(result2);
        }

       [Fact]
        public void Process_NewLineInsideParenthesesThenWhitespace_ReturnsEmitThenStepDown()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.BlockStart);
            sut.Process('a');
            sut.Process('\r');
            sut.Process('\n');
            sut.Process('a');
            sut.Process('\n');
            sut.Process('a');
            var result1 = sut.Process(Symbols.BlockEnd);
            var result2 = sut.Process('\n');
            sut.Process('a');

            // assert
            var emitResponse = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse);
            Assert.Equal("a\r\na\na", (emitResponse.Token as LiteralToken).Value);

            Assert.IsType<StepDownResponse>(result2);
        }

        [Fact]
        public void Process_ManyTokens_ReturnsTokens()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process('a');
            sut.Process('b');
            sut.Process('c');
            var result1 = sut.Process(' ');

            sut.Process('d');
            sut.Process('e');
            sut.Process('f');
            var result2 = sut.Process(' ');

            // assert
            var emitResponse1 = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse1);
            Assert.Equal("abc", (emitResponse1.Token as LiteralToken).Value);

            var emitResponse2 = result2 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse2);
            Assert.Equal("def", (emitResponse2.Token as LiteralToken).Value);
        }

        [Fact]
        public void Process_ManyParenthesisedTokens_ReturnsTokens()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.BlockStart);
            sut.Process('a');
            sut.Process('b');
            sut.Process('c');
            var result1 = sut.Process(Symbols.BlockEnd);

            sut.Process(Symbols.BlockStart);
            sut.Process('d');
            sut.Process('e');
            sut.Process('f');
            var result2 = sut.Process(Symbols.BlockEnd);

            // assert
            var emitResponse1 = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse1);
            Assert.Equal("abc", (emitResponse1.Token as LiteralToken).Value);

            var emitResponse2 = result2 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse2);
            Assert.Equal("def", (emitResponse2.Token as LiteralToken).Value);
        }

        [Fact]
        public void Process_EscapedParentheses_ReturnsTokenWithParentheses()
        {
            // arrange
            var sut = new ParseWordState();

            // act
            sut.Process(Symbols.Escape);
            sut.Process(Symbols.BlockStart);
            sut.Process(Symbols.Escape);
            sut.Process(Symbols.BlockEnd);
            var result1 = sut.Process(Symbols.BlockEnd);

            // assert
            var emitResponse1 = result1 as EmitAndStepDownResponse;
            Assert.NotNull(emitResponse1);
            Assert.Equal("()", (emitResponse1.Token as LiteralToken).Value);
        }

        // Unbalanced brackets

        /*[Fact]
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
        }*/

        /*[Fact]
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
        }*/
    }
}