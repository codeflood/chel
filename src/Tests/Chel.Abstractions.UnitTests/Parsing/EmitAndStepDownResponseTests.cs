using Xunit;
using System;
using Chel.Abstractions.Parsing;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class EmitAndStepDownResponseTests
    {
        [Fact]
        public void Ctor_TokenIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new EmitAndStepDownResponse(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("token", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var token = new EndOfBlockToken();
            
            // act
            var sut = new EmitAndStepDownResponse(token);

            // assert
            Assert.Equal(token, sut.Token);
        }
    }
}