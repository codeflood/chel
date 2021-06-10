using System;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class MalformedInputResponseTests
    {
        [Fact]
        public void Ctor_MessageIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new MalformedInputResponse(null);

            // act, assert
            Assert.Throws<ArgumentNullException>(sutAction);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void Ctor_MessageIsWhitespace_ThrowsException(string message)
        {
            // arrange
            Action sutAction = () => new MalformedInputResponse(message);

            // act, assert
            Assert.Throws<ArgumentException>(sutAction);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new MalformedInputResponse("message");

            // assert
            Assert.Equal("message", sut.Message);
        }
    }
}