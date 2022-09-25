using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Exceptions;
using Xunit;

namespace Chel.UnitTests.Exceptions
{
    public class ParseExceptionTests
    {
        [Fact]
        public void Ctor_WhenCalled_SetsSourceLine()
        {
            // arrange
            var location = new SourceLocation(42, 34);
            
            // act
            var sut = new ParseException(location, "message");

            // assert
            Assert.Equal(location, sut.SourceLocation);
            Assert.Equal("message", sut.Message);
        }
    }
}