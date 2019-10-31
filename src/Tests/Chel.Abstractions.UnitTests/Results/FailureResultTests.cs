using Xunit;
using Chel.Abstractions.Results;
using System;

namespace Chel.Abstractions.UnitTests.Results
{
    public class FailureResultTests
    {
        [Fact]
        public void Ctor_MessagesIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new FailureResult(1, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("messages", ex.ParamName);
        }

        [Fact]
        public void Ctor_MessagesIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new FailureResult(1, new string[0]);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("messages", ex.ParamName);
            Assert.Contains("messages cannot be empty", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsSuccessToFalse()
        {
            // act
            var sut = new FailureResult(1, new[]{ "message" });

            // assert
            Assert.False(sut.Success);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // act
            var sut = new FailureResult(3, new[]{ "message" });

            // assert
            Assert.Equal(3, sut.SourceLine);
            Assert.Equal(new[]{ "message" }, sut.Messages);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsExpectedString()
        {
            // arrange
            var sut = new FailureResult(3, new[] { "message" });

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("ERROR (Line 3): message", result);
        }

        [Fact]
        public void ToString_MultipleMessages_ReturnsEachMessageOnSeparateLine()
        {
            // arrange
            var sut = new FailureResult(3, new[] { "message", "message2" });

            // act
            var result = sut.ToString();

            // assert
            var nl = Environment.NewLine;
            Assert.Equal($"ERROR (Line 3):{nl}message{nl}message2", result);
        }
    }
}