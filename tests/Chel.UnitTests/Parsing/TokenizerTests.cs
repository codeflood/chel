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

        [Fact]
        public void GetNextToken_InputIsVariable_IncludesVariableToken()
        {
            // arrange
            var sut = new Tokenizer("$a$");

            var expected = new Token[] {
                new SpecialToken(new SourceLocation(1, 1), SpecialTokenType.VariableMarker),
                new LiteralToken(new SourceLocation(1, 2), 'a'),
                new SpecialToken(new SourceLocation(1, 3), SpecialTokenType.VariableMarker)
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputHasEscapedVariable_TreatVariableAsLiteral()
        {
            // arrange
            var sut = new Tokenizer("\\$a\\$");

            var expected = new[] {
                new LiteralToken(new SourceLocation(1, 2), '$'),
                new LiteralToken(new SourceLocation(1, 3), 'a'),
                new LiteralToken(new SourceLocation(1, 5), '$')
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputIncludesVariable_IncludesVariableToken()
        {
            // arrange
            var sut = new Tokenizer("a$a$");

            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new SpecialToken(new SourceLocation(1, 2), SpecialTokenType.VariableMarker),
                new LiteralToken(new SourceLocation(1, 3), 'a'),
                new SpecialToken(new SourceLocation(1, 4), SpecialTokenType.VariableMarker)
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputIncludesSeperatedVariable_IncludesVariableToken()
        {
            // arrange
            var sut = new Tokenizer("cmd $var$ cont");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'c'),
                new LiteralToken(new SourceLocation(1, 2), 'm'),
                new LiteralToken(new SourceLocation(1, 3), 'd'),
                new LiteralToken(new SourceLocation(1, 4), ' '),
                new SpecialToken(new SourceLocation(1, 5), SpecialTokenType.VariableMarker),
                new LiteralToken(new SourceLocation(1, 6), 'v'),
                new LiteralToken(new SourceLocation(1, 7), 'a'),
                new LiteralToken(new SourceLocation(1, 8), 'r'),
                new SpecialToken(new SourceLocation(1, 9), SpecialTokenType.VariableMarker),
                new LiteralToken(new SourceLocation(1, 10), ' '),
                new LiteralToken(new SourceLocation(1, 11), 'c'),
                new LiteralToken(new SourceLocation(1, 12), 'o'),
                new LiteralToken(new SourceLocation(1, 13), 'n'),
                new LiteralToken(new SourceLocation(1, 14), 't'),
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_BlockStartsWithDash_IncludesParameterNameSymbol()
        {
            // arrange
            var sut = new Tokenizer("cmd -par");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'c'),
                new LiteralToken(new SourceLocation(1, 2), 'm'),
                new LiteralToken(new SourceLocation(1, 3), 'd'),
                new LiteralToken(new SourceLocation(1, 4), ' '),
                new SpecialToken(new SourceLocation(1, 5), SpecialTokenType.ParameterName),
                new LiteralToken(new SourceLocation(1, 6), 'p'),
                new LiteralToken(new SourceLocation(1, 7), 'a'),
                new LiteralToken(new SourceLocation(1, 8), 'r'),
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_BlockStartsWithEscapedDash_DashTreatedAsLiteral()
        {
            // arrange
            var sut = new Tokenizer(@"cmd \-par");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'c'),
                new LiteralToken(new SourceLocation(1, 2), 'm'),
                new LiteralToken(new SourceLocation(1, 3), 'd'),
                new LiteralToken(new SourceLocation(1, 4), ' '),
                new LiteralToken(new SourceLocation(1, 6), '-'),
                new LiteralToken(new SourceLocation(1, 7), 'p'),
                new LiteralToken(new SourceLocation(1, 8), 'a'),
                new LiteralToken(new SourceLocation(1, 9), 'r'),
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_DashInParameter_IncludesParameterNameSymbol()
        {
            // arrange
            var sut = new Tokenizer(@"cmd p-ar");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'c'),
                new LiteralToken(new SourceLocation(1, 2), 'm'),
                new LiteralToken(new SourceLocation(1, 3), 'd'),
                new LiteralToken(new SourceLocation(1, 4), ' '),
                new LiteralToken(new SourceLocation(1, 5), 'p'),
                new SpecialToken(new SourceLocation(1, 6), SpecialTokenType.ParameterName),
                new LiteralToken(new SourceLocation(1, 7), 'a'),
                new LiteralToken(new SourceLocation(1, 8), 'r'),
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_DashAfterSpaceInBracketedParameter_IncludesParameterNameSymbol()
        {
            // arrange
            var sut = new Tokenizer(@"cmd (p -ar)");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'c'),
                new LiteralToken(new SourceLocation(1, 2), 'm'),
                new LiteralToken(new SourceLocation(1, 3), 'd'),
                new LiteralToken(new SourceLocation(1, 4), ' '),
                new SpecialToken(new SourceLocation(1, 5), SpecialTokenType.BlockStart),
                new LiteralToken(new SourceLocation(1, 6), 'p'),
                new LiteralToken(new SourceLocation(1, 7), ' '),
                new SpecialToken(new SourceLocation(1, 8), SpecialTokenType.ParameterName),
                new LiteralToken(new SourceLocation(1, 9), 'a'),
                new LiteralToken(new SourceLocation(1, 10), 'r'),
                new SpecialToken(new SourceLocation(1, 11), SpecialTokenType.BlockEnd)
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_ListInInput_IncludesListTokens()
        {
            // arrange
            var sut = new Tokenizer("[a b]");
            
            var expected = new Token[] {
                new SpecialToken(new SourceLocation(1, 1), SpecialTokenType.ListStart),
                new LiteralToken(new SourceLocation(1, 2), 'a'),
                new LiteralToken(new SourceLocation(1, 3), ' '),
                new LiteralToken(new SourceLocation(1, 4), 'b'),
                new SpecialToken(new SourceLocation(1, 5), SpecialTokenType.ListEnd)
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_EscapedListSymbolsInInput_IncludesListTokensAsLiterals()
        {
            // arrange
            var sut = new Tokenizer(@"\[a b\]");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 2), '['),
                new LiteralToken(new SourceLocation(1, 3), 'a'),
                new LiteralToken(new SourceLocation(1, 4), ' '),
                new LiteralToken(new SourceLocation(1, 5), 'b'),
                new LiteralToken(new SourceLocation(1, 7), ']')
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsSeparatedSubcommand_IncludesSubcommandTokens()
        {
            // arrange
            var sut = new Tokenizer("a << b");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), ' '),
                new SpecialToken(new SourceLocation(1, 3), SpecialTokenType.Subcommand),
                new LiteralToken(new SourceLocation(1, 5), ' '),
                new LiteralToken(new SourceLocation(1, 6), 'b')
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsJoinedSubcommand_IncludesSubcommandTokens()
        {
            // arrange
            var sut = new Tokenizer("a<<b");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new SpecialToken(new SourceLocation(1, 2), SpecialTokenType.Subcommand),
                new LiteralToken(new SourceLocation(1, 4), 'b')
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsEscapedSubcommandFirstElement_IncludesSubcommandTokensAsLiterals()
        {
            // arrange
            var sut = new Tokenizer(@"a \<< b");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), ' '),
                new LiteralToken(new SourceLocation(1, 4), '<'),
                new LiteralToken(new SourceLocation(1, 5), '<'),
                new LiteralToken(new SourceLocation(1, 6), ' '),
                new LiteralToken(new SourceLocation(1, 7), 'b')
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsEscapedSubcommandSecondElement_IncludesSubcommandTokensAsLiterals()
        {
            // arrange
            var sut = new Tokenizer(@"a <\< b");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), ' '),
                new LiteralToken(new SourceLocation(1, 3), '<'),
                new LiteralToken(new SourceLocation(1, 5), '<'),
                new LiteralToken(new SourceLocation(1, 6), ' '),
                new LiteralToken(new SourceLocation(1, 7), 'b')
            };

            // act, assert
            AssertTokenStream(expected, sut);
        }

        [Fact]
        public void GetNextToken_InputContainsSingleLessThanCharacter_IncludesLiteralInOutput()
        {
            // arrange
            var sut = new Tokenizer("a < b");
            
            var expected = new Token[] {
                new LiteralToken(new SourceLocation(1, 1), 'a'),
                new LiteralToken(new SourceLocation(1, 2), ' '),
                new LiteralToken(new SourceLocation(1, 3), '<'),
                new LiteralToken(new SourceLocation(1, 4), ' '),
                new LiteralToken(new SourceLocation(1, 5), 'b')
            };

            // act, assert
            AssertTokenStream(expected, sut);
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
