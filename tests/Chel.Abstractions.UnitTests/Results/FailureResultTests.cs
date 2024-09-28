using Xunit;
using Chel.Abstractions.Results;
using System;

namespace Chel.Abstractions.UnitTests.Results
{
    public class FailureResultTests
    {   
        [Fact]
        public void Ctor_MessageIsNull_ThrowsException()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            Action sutAction = () => new FailureResult(location, null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("message", ex.ParamName);
        }

        [Fact]
        public void Ctor_MessageIsEmpty_ThrowsException()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            Action sutAction = () => new FailureResult(location, "");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("message", ex.ParamName);
            Assert.Contains("'message' cannot be empty or whitespace", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsSuccessToFalse()
        {
            // act
            var location = new SourceLocation(1, 1);
            var sut = new FailureResult(location, "message");

            // assert
            Assert.False(sut.Success);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // act
            var location = new SourceLocation(2, 4);
            var sut = new FailureResult(location, "message");

            // assert
            Assert.Equal(location, sut.SourceLocation);
            Assert.Equal("message", sut.Message);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsExpectedString()
        {
            // arrange
            var location = new SourceLocation(3, 1);
            var sut = new FailureResult(location, "message");

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("ERROR (line 3, character 1): message", result);
        }
    }
}