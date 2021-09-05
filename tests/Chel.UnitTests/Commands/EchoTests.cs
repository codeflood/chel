using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Commands;
using Xunit;

namespace Chel.UnitTests.Commands
{
    public class EchoTests
    {
        [Fact]
        public void Execute_MessageIsNull_ReturnsEmptyString()
        {
            // arrange
            var sut = new Echo();
            sut.Message = null;

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public void Execute_MessagePopulated_ReturnsMessage()
        {
            // arrange
            var sut = new Echo();
            sut.Message = new Literal("message");

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.True(result.Success);
            Assert.Equal("message", result.Value);
        }
    }
}