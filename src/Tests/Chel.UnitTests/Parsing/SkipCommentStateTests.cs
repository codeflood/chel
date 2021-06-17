using Chel.Abstractions.Parsing;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
    public class SkipCommentStateTests
    {
        [Theory]
        [InlineData('a')]
        [InlineData('A')]
        [InlineData(' ')]
        [InlineData('\n')]
        [InlineData(Symbols.BlockStart)]
        [InlineData(Symbols.ParameterName)]
        [InlineData(Symbols.Variable)]
        public void CanProcess_InputIsNonCommentSymbol_ReturnsFalse(char input)
        {
            // arrange
            var sut = new SkipCommentState();

            // act
            var result = sut.CanProcess(input);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void CanProcess_InputIsCommentSymbol_ReturnsTrue()
        {
            // arrange
            var sut = new SkipCommentState();

            // act
            var result = sut.CanProcess(Symbols.Comment);

            // assert
            Assert.True(result);
        }

        [Theory]
        [InlineData('a')]
        [InlineData('A')]
        [InlineData(' ')]
        [InlineData(Symbols.BlockEnd)]
        [InlineData(Symbols.BlockStart)]
        [InlineData(Symbols.Comment)]
        [InlineData(Symbols.ParameterName)]
        [InlineData(Symbols.Variable)]
        public void Process_InputIsNotNewline_ReturnsContinue(char input)
        {
            // arrange
            var sut = new SkipCommentState();

            // act
            var result = sut.Process(input);

            // assert
            Assert.IsType<ContinueResponse>(result);
        }

        [Fact]
        public void Process_InputIsNewline_ReturnsStepDown()
        {
            // arrange
            var sut = new SkipCommentState();

            // act
            var result = sut.Process('\n');

            // assert
            Assert.IsType<StepDownResponse>(result);
        }
    }
}