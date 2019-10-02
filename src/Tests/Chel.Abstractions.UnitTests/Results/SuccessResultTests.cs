using Xunit;
using Chel.Abstractions.Results;

namespace Chel.Abstractions.UnitTests.Results
{
    public class SuccessResultTests
    {
        [Fact]
        public void ToString_WhenCalled_ReturnsExpectedString()
        {
            // arrange
            var sut = new SuccessResult();

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("", result);
        }
    }
}