using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions.Parsing;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
    public class ParseMultiWordStateTests
    {
        [Theory]
        [InlineData('a')]
        [InlineData(Symbols.Comment)]
        [InlineData(Symbols.Escape)]
        [InlineData(Symbols.ParameterName)]
        [InlineData(Symbols.Variable)]
        public void Process_InputIsNotBlockStartSymbol_ReturnsEmptyToken(char input)
        {
            // arrange
            var sut = new ParseMultiWordState();

            // act
            var result = sut.Process(input);

            // assert
            var setStateResposne = Assert.IsType<SetStateResponse>(result.First());
            Assert.IsType<SkipWhiteSpaceState>(setStateResposne.State);
            Assert.True(setStateResposne.Reprocess);
        }

        [Theory]
        [InlineData("()", "")]
        [InlineData("(a)", "a")]
        [InlineData("(input)", "input")]
        [InlineData("(with space)", "with space")]
        public void Process_InputIsValidMultiWord_ReturnsMultiWordAndSetsStateToSkipWhitespace(string input, string expected)
        {
            // arrange
            var sut = new ParseMultiWordState();
            IEnumerable<TokenizerStateResponse> result = null;
            var continueResponseCount = 0;

            // act
            foreach(var c in input)
            {
                result = sut.Process(c);
                if(result.First() is ContinueResponse)
                    continueResponseCount++;
            }

            // assert
            Assert.Equal(input.Length - 1, continueResponseCount);

            var emitResposne = Assert.IsType<EmitResponse>(result.First());
            var token = Assert.IsType<LiteralToken>(emitResposne.Token);
            Assert.Equal(expected, token.Value);

            var setStateResposne = Assert.IsType<SetStateResponse>(result.ElementAt(1));
            Assert.IsType<SkipWhiteSpaceState>(setStateResposne.State);
            Assert.True(setStateResposne.Reprocess);
        }

        // missing end bracket
        [Fact]
        public void Process_MissingEndBracket_ReturnsMalformedInput()
        {
            // arrange
            var sut = new ParseMultiWordState();

            // act
            sut.Process(Symbols.BlockStart);
            sut.Process('a');
            sut.Process('b');
            sut.Process('\n');
            sut.Process('c');

            // assert
            var resposne = Assert.IsType<EmitResponse>(result.First());
            var token = Assert.IsType<LiteralToken>(emitResposne.Token);
            Assert.Equal(expected, token.Value);

            var setStateResposne = Assert.IsType<SetStateResponse>(result.ElementAt(1));
            Assert.IsType<SkipWhiteSpaceState>(setStateResposne.State);
            Assert.True(setStateResposne.Reprocess);
        }

        // nested brackets

        // escaped bracket
    }
}
