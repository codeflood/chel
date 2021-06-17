using System;
using Chel.Abstractions.Parsing;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
    public class TokenizerTests
    {
        [Fact]
        public void GetNextToken_InputIsNull_ReturnsEobThenNull()
        {
            // arrange
            var sut = new Tokenizer(null);

            // act
            var token1 = sut.GetNextToken();
            var token2 = sut.GetNextToken();

            // assert
            Assert.IsType<EndOfBlockToken>(token1);
            Assert.Null(token2);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("     ")]
        [InlineData("\t")]
        [InlineData("\r\n")]
        [InlineData("\n")]
        public void GetNextToken_InputIsWhitespaceOrNewLine_ReturnsEob(string input)
        {
            // arrange
            var sut = new Tokenizer(input);

            // act
            var token = sut.GetNextToken();

            // assert
            Assert.IsType<EndOfBlockToken>(token);
        }

        [Fact]
        public void GetNextToken_InputIsWord_ReturnsValueTokenThenEobThenNull()
        {
            // arrange
            var sut = new Tokenizer("cmd");

            // act
            var token1 = sut.GetNextToken();
            var token2 = sut.GetNextToken();
            var token3 = sut.GetNextToken();

            // assert
            var valueToken = token1 as LiteralToken;
            Assert.NotNull(valueToken);
            Assert.Equal("cmd", valueToken.Value);

            Assert.IsType<EndOfBlockToken>(token2);
            Assert.Null(token3);
        }

        [Theory]
        [InlineData(" cmd ")]
        [InlineData("\tcmd\r\n")]
        [InlineData("   cmd  \t\r\n")]
        [InlineData(" cmd\r\n\r\n")]
        public void GetNextToken_InputIsWordWithSurroundingWhiteSpace_ReturnsValueToken(string input)
        {
            // arrange
            var sut = new Tokenizer(input);

            // act
            var token = sut.GetNextToken();

            // assert
            var valueToken = token as LiteralToken;
            Assert.NotNull(valueToken);
            Assert.Equal("cmd", valueToken.Value);
        }

        [Fact]
        public void GetNextToken_WordWithCommentNoWhiteSpace_ReturnsValueToken()
        {
            // arrange
            var sut = new Tokenizer("cmd#comment");

            // act
            var token = sut.GetNextToken();

            // assert
            var valueToken = token as LiteralToken;
            Assert.NotNull(valueToken);
            Assert.Equal("cmd", valueToken.Value);
        }
    }
}
