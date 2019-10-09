using Xunit;
using Chel.Abstractions.Results;

namespace Chel.Abstractions.UnitTests.Results
{
    public class UnknownCommandResultTests
    {
        [Fact]
        public void ToString_WhenCalled_ReturnsExpectedString()
        {
            // arrange
            var sut = new UnknownCommandResult(3);

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("ERROR (Line 3): Unknown command", result);
        }
    }
}