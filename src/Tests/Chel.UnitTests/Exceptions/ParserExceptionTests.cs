using Chel.Exceptions;
using Xunit;

namespace Chel.UnitTests.Exceptions
{
    public class ParserExceptionTests
    {
        [Fact]
        public void Ctor_WhenCalled_SetsSourceLine()
        {
            // arrange, act
            var sut = new ParserException(3, "message");

            // assert
            Assert.Equal(3, sut.SourceLine);
        }
    }
}