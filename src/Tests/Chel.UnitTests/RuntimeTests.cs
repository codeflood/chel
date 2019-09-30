using System;
using Chel.Abstractions;
using Xunit;

namespace Chel.UnitTests
{
    public class RuntimeTests
    {
        [Fact]
        public void RegisterCommandType_TypeIsNull_ThrowsException()
        {
            // arrange
            var sut = new Runtime();
            Action sutAction = () => sut.RegisterCommandType(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandType", ex.ParamName);
        }

        [Fact]
        public void NewSession_WhenCalled_ReturnsSession()
        {
            // arrange
            var sut = new Runtime();

            // act
            var session = sut.NewSession();

            // assert
            Assert.NotNull(session);
        }
    }
}