using System.Collections.Generic;
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
        public void HasMore_InputIsNullOrEmpty_ReturnsFalse(string input)
        {
            // arrange
            var sut = new Tokenizer(input);

            // act
            var result = sut.HasMore;

            // assert
            Assert.False(result);
        }

        [Fact]
        public void HasMore_InputNotRead_ReturnsTrue()
        {
            // arrange
            var sut = new Tokenizer("abc");

            // act
            var result = sut.HasMore;

            // assert
            Assert.True(result);
        }

        [Fact]
        public void HasMore_InputHasBeenRead_ReturnsFalse()
        {
            // arrange
            var sut = new Tokenizer("abc");

            // act
            sut.GetNextToken();
            sut.GetNextToken();
            sut.GetNextToken();
            sut.GetNextToken();
            var result = sut.HasMore;

            // assert
            Assert.False(result);
        }

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
            var expected = new[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), 'b'),
                new LiteralToken(new SourceLocation(1, 3), 'c')
            };
            
            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputIncludesNewline_LineNumbersAreCorrect()
        {
            // arrange
            var sut = new Tokenizer("\na\nb\n");

            var expected = new[] {
                new LiteralToken(new SourceLocation(1, 1), '\n'),
                new LiteralToken(new SourceLocation(2, 1), 'a'),
                new LiteralToken(new SourceLocation(2, 2), '\n'),
                new LiteralToken(new SourceLocation(3, 1), 'b'),
                new LiteralToken(new SourceLocation(3, 2), '\n')
            };
            
            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsBlock_ReturnsSpecialTokens()
        {
            // arrange
            var sut = new Tokenizer("(a)");

            var expected = new Token[] {
                new SpecialToken(new SourceLocation(1, 1), SpecialTokenType.BlockStart),
                new LiteralToken(new SourceLocation(1, 2), 'a'),
                new SpecialToken(new SourceLocation(1, 3), SpecialTokenType.BlockEnd)
            };
            
            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsEscapedSpecialCharacters_ReturnsLiteralsTokens()
        {
            // arrange
            var sut = new Tokenizer("\\(a\\)");

            var expected = new[] {
                new LiteralToken(new SourceLocation(1, 2), '('),
                new LiteralToken(new SourceLocation(1, 3), 'a'),
                new LiteralToken(new SourceLocation(1, 5), ')')
            };
            
            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsEscapedNormalCharacters_ThrowsException()
        {
            // arrange
            var sut = new Tokenizer("\\a");
            
            // act
            var ex = Assert.Throws<ParseException>(() => sut.GetNextToken());
            Assert.Equal("Unknown escaped character '\\a'", ex.Message);
            
            var sourceLocation = new SourceLocation(1, 1);
            Assert.Equal(sourceLocation, ex.SourceLocation);
        }

        [Fact]
        public void GetNextToken_InputContainsEscapedEscapeCharacter_ReturnsLiteralsToken()
        {
            // arrange
            var sut = new Tokenizer("a\\\\");

            var expected = new[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 3), '\\')
            };
            
            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsEscapedCommentCharacter_ReturnsLiteralsToken()
        {
            // arrange
            var sut = new Tokenizer("a \\# b");

            var expected = new[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), ' '),
                new LiteralToken(new SourceLocation(1, 4), '#'),
                new LiteralToken(new SourceLocation(1, 5), ' '),
                new LiteralToken(new SourceLocation(1, 6), 'b')
            };
            
            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsComment_ExcludesComment()
        {
            // arrange
            var sut = new Tokenizer("abc #comment");

            var expected = new[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), 'b'),
                new LiteralToken(new SourceLocation(1, 3), 'c'),
                new LiteralToken(new SourceLocation(1, 4), ' ')
            };
            
            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsCommentInsideBlock_ExcludesComment()
        {
            // arrange
            var sut = new Tokenizer("ab (c #comment)");

            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), 'b'),
                new LiteralToken(new SourceLocation(1, 3), ' '),
                new SpecialToken(new SourceLocation(1, 4), SpecialTokenType.BlockStart),
                new LiteralToken(new SourceLocation(1, 5), 'c'),
                new LiteralToken(new SourceLocation(1, 6), ' '),
            };
            
            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsCommentBlock_ExcludesCommentBlock()
        {
            // arrange
            var sut = new Tokenizer("ab(#comment#)c");

            var expected = new[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), 'b'),
                new LiteralToken(new SourceLocation(1, 14), 'c')
            };
            
            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_CommentBlockMissingEnd_ThrowsException()
        {
            // arrange
            var sut = new Tokenizer("ab (#comment");

            var expected = new[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), 'b'),
                new LiteralToken(new SourceLocation(1, 3), ' ')
            };
            
            // act, assert
            AssertTokenStream(expected, sut, expectError: true);

            var ex = Assert.Throws<ParseException>(() => sut.GetNextToken());
            Assert.Equal("Missing comment block end.", ex.Message);

            var commentBlockStartLocation = new SourceLocation(1, 4);
            Assert.Equal(commentBlockStartLocation, ex.SourceLocation);
        }

        [Fact]
        public void GetNextToken_CommentBlockStartMissing_ThrowsException()
        {
            // arrange
            var sut = new Tokenizer("ab #)");

            var expected = new[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), 'b'),
                new LiteralToken(new SourceLocation(1, 3), ' ')
            };
            
            // act, assert
            AssertTokenStream(expected, sut, expectError: true);
            
            var ex = Assert.Throws<ParseException>(() => sut.GetNextToken());
            Assert.Equal("Missing comment block start.", ex.Message);

            var commentBlockStartLocation = new SourceLocation(1, 4);
            Assert.Equal(commentBlockStartLocation, ex.SourceLocation);
        }

        private void AssertTokenStream(IEnumerable<Token> expected, Tokenizer sut, bool expectError = false)
        {
            // act, assert
            foreach(var token in expected)
            {
                var result = sut.GetNextToken();
                Assert.Equal(token, result);
            }

            if(!expectError)
            {
                var finalResult = sut.GetNextToken();
                Assert.Null(finalResult);
            }
        }
    }
}
