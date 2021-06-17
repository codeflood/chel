using Xunit;
using Chel.Parsing;
using Chel.Abstractions.Parsing;

namespace Chel.UnitTests.Parsing
{
	public class SkipWhiteSpaceStateTests
    {
        [Theory]
        [InlineData('a')]
        [InlineData('A')]
        [InlineData(Symbols.Variable)]
        [InlineData(Symbols.BlockStart)]
        [InlineData(Symbols.BlockEnd)]
        [InlineData(Symbols.Comment)]
        public void CanProcess_InputIsNotWhiteSpace_ReturnsFalse(char input)
        {
            // arrange
            var sut = new SkipWhiteSpaceState();

            // act
            var result = sut.CanProcess(input);

            // assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(' ')]
        [InlineData('\t')]
        [InlineData('\r')]
        public void CanProcess_InputIsWhiteSpace_ReturnsTrue(char input)
        {
            // arrange
            var sut = new SkipWhiteSpaceState();

            // act
            var result = sut.CanProcess(input);

            // assert
            Assert.True(result);
        }

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
            Assert.IsType<ContinueResponse>(result);
        }

        [Theory]
        [InlineData('a')]
        [InlineData('A')]
        [InlineData(Symbols.Variable)]
        [InlineData(Symbols.BlockStart)]
        [InlineData(Symbols.BlockEnd)]
        [InlineData(Symbols.Comment)]
        public void Process_InputIsNotWhiteSpace_ReturnsStepDown(char input)
        {
            // arrange
            var sut = new SkipWhiteSpaceState();

            // act
            var result = sut.Process(input);

            // assert
            Assert.IsType<StepDownResponse>(result);
        }

        [Fact]
        public void Process_InputIsNewLine_ReturnsEmitWithEndOfBlock()
        {
            // arrange
            var sut = new SkipWhiteSpaceState();

            // act
            var result = sut.Process('\n');

            // assert
            Assert.IsType<EmitAndStepDownResponse>(result);

            var emitResponse = (EmitAndStepDownResponse)result;
            Assert.IsType<EndOfBlockToken>(emitResponse.Token);
        }
    }
}