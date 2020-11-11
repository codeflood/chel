using System;
using Chel.Abstractions.Parsing;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests
{
    public class TokenizerTests
    {
        [Fact]
        public void GetNextToken_InputIsNull_ReturnsNull()
        {
            // arrange
            var sut = new Tokenizer(null);

            // act
            var token = sut.GetNextToken();

            // assert
            Assert.Null(token);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("     ")]
        [InlineData("\t")]
        public void GetNextToken_InputIsWhitespace_ReturnsNull(string input)
        {
            // arrange
            var sut = new Tokenizer(input);

            // act
            var token = sut.GetNextToken();

            // assert
            Assert.Null(token);
        }

        [Theory]
        [InlineData("\r\n")]
        [InlineData("\n")]
        public void GetNextToken_InputIsNewline_ReturnsEob(string input)
        {
            // arrange
            var sut = new Tokenizer(input);

            // act
            var token = sut.GetNextToken();

            // assert
            Assert.IsType<EndOfBlockToken>(token);
        }
    }
}
