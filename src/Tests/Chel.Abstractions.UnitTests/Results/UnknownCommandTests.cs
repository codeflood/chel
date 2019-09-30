using Xunit;
using Chel.Abstractions.Results;

namespace Chel.Abstractions.UnitTests.Results
{
    public class UnknownCommandTests
    {
        [Fact]
        public void ToString_WhenCalled_ReturnsExpectedString()
        {
            // arrange
            var sut = new UnknownCommand();

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("Unknown command", result);
        }
    }
}