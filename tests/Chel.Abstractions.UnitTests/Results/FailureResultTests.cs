using Xunit;
using Chel.Abstractions.Results;
using System;
using Chel.Abstractions.Parsing;

namespace Chel.Abstractions.UnitTests.Results
{
    public class FailureResultTests
    {   
        [Fact]
        public void Ctor_MessagesIsNull_ThrowsException()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            Action sutAction = () => new FailureResult(location, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("messages", ex.ParamName);
        }

        [Fact]
        public void Ctor_MessagesIsEmpty_ThrowsException()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            Action sutAction = () => new FailureResult(location, new string[0]);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("messages", ex.ParamName);
            Assert.Contains("'messages' cannot be empty", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsSuccessToFalse()
        {
            // act
            var location = new SourceLocation(1, 1);
            var sut = new FailureResult(location, new[]{ "message" });

            // assert
            Assert.False(sut.Success);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // act
            var location = new SourceLocation(2, 4);
            var sut = new FailureResult(location, new[]{ "message" });

            // assert
            Assert.Equal(location, sut.SourceLocation);
            Assert.Equal(new[]{ "message" }, sut.Messages);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsExpectedString()
        {
            // arrange
            var location = new SourceLocation(3, 1);
            var sut = new FailureResult(location, new[] { "message" });

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("ERROR (line 3, character 1): message", result);
        }

        [Fact]
        public void ToString_MultipleMessages_ReturnsEachMessageOnSeparateLine()
        {
            // arrange
            var location = new SourceLocation(3, 6);
            var sut = new FailureResult(location, new[] { "message", "message2" });

            // act
            var result = sut.ToString();

            // assert
            var nl = Environment.NewLine;
            Assert.Equal($"ERROR (line 3, character 6):{nl}message{nl}message2", result);
        }
    }
}