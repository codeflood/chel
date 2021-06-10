using System;
using Chel.Abstractions.Parsing;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class PushStateResponseTests
    {
        [Fact]
        public void Ctor_StateIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new PushStateResponse(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("state", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var state = Substitute.For<ITokenizerState>();

            // act
            var sut = new PushStateResponse(state);

            // assert
            Assert.Equal(state, sut.State);
        }
    }
}
