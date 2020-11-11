using System;
using Chel.Abstractions.Parsing;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class SetStateResponseTests
    {
        [Fact]
        public void Ctor_StateIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new SetStateResponse(null, false);

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
            var sut = new SetStateResponse(state, true);

            // assert
            Assert.Equal(state, sut.State);
            Assert.True(sut.Reprocess);
        }
    }
}