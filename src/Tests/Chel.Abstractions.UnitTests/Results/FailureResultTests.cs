using Xunit;
using Chel.Abstractions.Results;

namespace Chel.Abstractions.UnitTests.Results
{
    public class FailureResultTests
    {
        [Fact]
        public void Ctor_WhenCalled_SetsSuccessToFalse()
        {
            // act
            var sut = new FailureResult(1);

            // assert
            Assert.False(sut.Success);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // act
            var sut = new FailureResult(3);

            // assert
            Assert.Equal(3, sut.SourceLine);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsExpectedString()
        {
            // arrange
            var sut = new FailureResult(3);

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("ERROR (Line 3)", result);
        }
    }
}