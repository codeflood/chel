using Xunit;
using Chel.Parsing;
using Chel.Abstractions.Parsing;
using System.Linq;

namespace Chel.UnitTests.Parsing
{
    public class SkipWhiteSpaceStateTests
    {
        [Theory]
        [InlineData(' ')]
        [InlineData('\t')]
        [InlineData('\r')]
        public void Process_InputIsWhiteSpace_ReturnsContinueResponse(char input)
        {
            // arrange
            var sut = new SkipWhiteSpaceState();

            // act
            var result = sut.Process(input);

            // assert
            Assert.IsType<ContinueResponse>(result.Single());
        }

        [Theory]
        [InlineData('a')]
        [InlineData('A')]
        [InlineData(Symbols.Variable)]
        [InlineData(Symbols.BlockStart)]
        [InlineData(Symbols.BlockEnd)]
        [InlineData(Symbols.Comment)]
        public void Process_InputIsNotWhiteSpace_ReturnsSetStateResponse(char input)
        {
            // arrange
            var sut = new SkipWhiteSpaceState();

            // act
            var result = sut.Process(input);

            // assert
            Assert.IsType<SetStateResponse>(result.Single());
            
            var setStateResponse = (SetStateResponse)result.Single();
            Assert.IsType<ParseWordState>(setStateResponse.State);
            Assert.True(setStateResponse.Reprocess);
        }

        [Fact]
        public void Process_InputIsNewLine_ReturnsEmitWithEndOfBlock()
        {
            // arrange
            var sut = new SkipWhiteSpaceState();

            // act
            var result = sut.Process('\n');

            // assert
            Assert.IsType<EmitResponse>(result.Single());

            var emitResponse = (EmitResponse)result.Single();
            Assert.IsType<EndOfBlockToken>(emitResponse.Token);
        }
    }
}