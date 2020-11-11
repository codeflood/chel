using Xunit;
using Chel.Abstractions.Parsing;
using System.Collections.Generic;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class CompositeTokenTests
    {
        [Fact]
        public void Ctor_TokensIsNull_SetsPropertyToEmpty()
        {
            // arrange, act
            var sut = new CompositeToken(null);

            // assert
            Assert.Empty(sut.Tokens);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var token1 = new LiteralToken("lit");
            var token2 = new VariableToken("var");
            var tokens = new List<Token> { token1, token2 };
            
            // act
            var sut = new CompositeToken(tokens);

            // assert
            Assert.Equal(tokens, sut.Tokens);
        }

        [Fact]
        public void Tokens_SourceChangedAfterCall_TokensDoesNotChange()
        {
            // arrange
            var token1 = new LiteralToken("lit");
            var token2 = new VariableToken("var");
            var tokens = new List<Token> { token1 };
            var sut = new CompositeToken(tokens);

            // act
            tokens.Add(token2);

            // assert
            Assert.Equal(new[]{ token1 }, sut.Tokens);
        }
    }
}
