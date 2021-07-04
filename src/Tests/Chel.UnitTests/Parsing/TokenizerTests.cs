using Chel.Abstractions.Parsing;
using Chel.Exceptions;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
	public class TokenizerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetNextToken_InputIsNullOrEmpty_ReturnsNull(string input)
        {
            // arrange
            var sut = new Tokenizer(input);

            // act
            var result = sut.GetNextToken();

            // assert
            Assert.Null(result);
        }

        [Fact]
        public void GetNextToken_InputIsOnlyLiterals_ReturnsOnlyLiterals()
        {
            // arrange
            var sut = new Tokenizer("abc");
            
            // act
            var result1 = (LiteralToken)sut.GetNextToken();
            var result2 = (LiteralToken)sut.GetNextToken();
            var result3 = (LiteralToken)sut.GetNextToken();
            var result4 = sut.GetNextToken();

            // assert
            Assert.Equal('a', result1.Value);
            Assert.Equal(new SourceLocation(1, 1), result1.Location);
            Assert.Equal('b', result2.Value);
            Assert.Equal(new SourceLocation(1, 2), result2.Location);
            Assert.Equal('c', result3.Value);
            Assert.Equal(new SourceLocation(1, 3), result3.Location);
            Assert.Null(result4);
        }

        [Fact]
        public void GetNextToken_InputIncludesNewline_LineNumbersAreCorrect()
        {
            // arrange
            var sut = new Tokenizer("\na\nb\n");
            
            // act
            var result1 = (LiteralToken)sut.GetNextToken();
            var result2 = (LiteralToken)sut.GetNextToken();
            var result3 = (LiteralToken)sut.GetNextToken();
            var result4 = (LiteralToken)sut.GetNextToken();
            var result5 = (LiteralToken)sut.GetNextToken();
            var result6 = sut.GetNextToken();

            // assert
            Assert.Equal('\n', result1.Value);
            Assert.Equal(new SourceLocation(1, 1), result1.Location);
            Assert.Equal('a', result2.Value);
            Assert.Equal(new SourceLocation(2, 1), result2.Location);
            Assert.Equal('\n', result3.Value);
            Assert.Equal(new SourceLocation(2, 2), result3.Location);
            Assert.Equal('b', result4.Value);
            Assert.Equal(new SourceLocation(3, 1), result4.Location);
            Assert.Equal('\n', result5.Value);
            Assert.Equal(new SourceLocation(3, 2), result5.Location);
            Assert.Null(result6);
        }

        [Fact]
        public void GetNextToken_InputContainsBlock_ReturnsSpecialTokens()
        {
            // arrange
            var sut = new Tokenizer("(a)");
            
            // act
            var result1 = (SpecialToken)sut.GetNextToken();
            var result2 = (LiteralToken)sut.GetNextToken();
            var result3 = (SpecialToken)sut.GetNextToken();
            var result4 = sut.GetNextToken();

            // assert
            Assert.Equal(SpecialTokenType.BlockStart, result1.Type);
            Assert.Equal(new SourceLocation(1, 1), result1.Location);
            Assert.Equal('a', result2.Value);
            Assert.Equal(new SourceLocation(1, 2), result2.Location);
            Assert.Equal(SpecialTokenType.BlockEnd, result3.Type);
            Assert.Equal(new SourceLocation(1, 3), result3.Location);
            Assert.Null(result4);
        }

        [Fact]
        public void GetNextToken_InputContainsEscapedSpecialCharacters_ReturnsLiteralsTokens()
        {
            // arrange
            var sut = new Tokenizer("\\(a\\)");
            
            // act
            var result1 = (LiteralToken)sut.GetNextToken();
            var result2 = (LiteralToken)sut.GetNextToken();
            var result3 = (LiteralToken)sut.GetNextToken();
            var result4 = sut.GetNextToken();

            // assert
            Assert.Equal('(', result1.Value);
            Assert.Equal(new SourceLocation(1, 2), result1.Location);
            Assert.Equal('a', result2.Value);
            Assert.Equal(new SourceLocation(1, 3), result2.Location);
            Assert.Equal(')', result3.Value);
            Assert.Equal(new SourceLocation(1, 5), result3.Location);
            Assert.Null(result4);
        }

        [Fact]
        public void GetNextToken_InputContainsEscapedNormalCharacters_ThrowsException()
        {
            // arrange
            var sut = new Tokenizer("\\a");
            
            // act
            var ex = Assert.Throws<ParserException>(() => sut.GetNextToken());
        }

        [Fact]
        public void GetNextToken_InputContainsEscapedEscapeCharacter_ReturnsLiteralsToken()
        {
            // arrange
            var sut = new Tokenizer("a\\\\");
            
            // act
            var result1 = (LiteralToken)sut.GetNextToken();
            var result2 = (LiteralToken)sut.GetNextToken();
            var result3 = sut.GetNextToken();

            // assert
            Assert.Equal('a', result1.Value);
            Assert.Equal(new SourceLocation(1, 1), result1.Location);
            Assert.Equal('\\', result2.Value);
            Assert.Equal(new SourceLocation(1, 3), result2.Location);
            Assert.Null(result3);
        }

        [Fact]
        public void GetNextToken_InputContainsEscapedCommentCharacter_ReturnsLiteralsToken()
        {
            // arrange
            var sut = new Tokenizer("a \\# b");

            // act
            var result1 = (LiteralToken)sut.GetNextToken();
            var result2 = (LiteralToken)sut.GetNextToken();
            var result3 = (LiteralToken)sut.GetNextToken();
            var result4 = (LiteralToken)sut.GetNextToken();
            var result5 = (LiteralToken)sut.GetNextToken();
            var result6 = sut.GetNextToken();

            // assert
            Assert.Equal('a', result1.Value);
            Assert.Equal(new SourceLocation(1, 1), result1.Location);
            Assert.Equal(' ', result2.Value);
            Assert.Equal(new SourceLocation(1, 2), result2.Location);
            Assert.Equal('#', result3.Value);
            Assert.Equal(new SourceLocation(1, 4), result3.Location);
            Assert.Equal(' ', result4.Value);
            Assert.Equal(new SourceLocation(1, 5), result4.Location);
            Assert.Equal('b', result5.Value);
            Assert.Equal(new SourceLocation(1, 6), result5.Location);
            Assert.Null(result6);
        }

        [Fact]
        public void GetNextToken_InputContainsComment_ExcludesComment()
        {
            // arrange
            var sut = new Tokenizer("abc #comment");

            // act
            var result1 = (LiteralToken)sut.GetNextToken();
            var result2 = (LiteralToken)sut.GetNextToken();
            var result3 = (LiteralToken)sut.GetNextToken();
            var result4 = (LiteralToken)sut.GetNextToken();
            var result5 = sut.GetNextToken();

            // assert
            Assert.Equal('a', result1.Value);
            Assert.Equal(new SourceLocation(1, 1), result1.Location);
            Assert.Equal('b', result2.Value);
            Assert.Equal(new SourceLocation(1, 2), result2.Location);
            Assert.Equal('c', result3.Value);
            Assert.Equal(new SourceLocation(1, 3), result3.Location);
            Assert.Equal(' ', result4.Value);
            Assert.Equal(new SourceLocation(1, 4), result4.Location);
            Assert.Null(result5);
        }

        [Fact]
        public void GetNextToken_InputContainsCommentInsideBlock_ExcludesComment()
        {
            // arrange
            var sut = new Tokenizer("ab (c #comment)");

            // act
            var result1 = (LiteralToken)sut.GetNextToken();
            var result2 = (LiteralToken)sut.GetNextToken();
            var result3 = (LiteralToken)sut.GetNextToken();
            var result4 = (SpecialToken)sut.GetNextToken();
            var result5 = (LiteralToken)sut.GetNextToken();
            var result6 = (LiteralToken)sut.GetNextToken();
            var result7 = sut.GetNextToken();

            // assert
            Assert.Equal('a', result1.Value);
            Assert.Equal(new SourceLocation(1, 1), result1.Location);
            Assert.Equal('b', result2.Value);
            Assert.Equal(new SourceLocation(1, 2), result2.Location);
            Assert.Equal(' ', result3.Value);
            Assert.Equal(new SourceLocation(1, 3), result3.Location);
            Assert.Equal(SpecialTokenType.BlockStart, result4.Type);
            Assert.Equal(new SourceLocation(1, 4), result4.Location);
            Assert.Equal('c', result5.Value);
            Assert.Equal(new SourceLocation(1, 5), result5.Location);
            Assert.Equal(' ', result6.Value);
            Assert.Equal(new SourceLocation(1, 6), result6.Location);
            Assert.Null(result7);
        }
    }
}
