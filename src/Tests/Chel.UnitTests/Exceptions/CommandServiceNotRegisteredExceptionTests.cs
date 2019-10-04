using System;
using Chel.UnitTests.Services;
using Xunit;

namespace Chel.UnitTests.Exceptions
{
    public class CommandServiceNotRegisteredExceptionTests
    {
        [Fact]
        public void Ctor_TypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandServiceNotRegisteredException(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("type", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsMessageProperty()
        {
            // arrange, act
            var sut = new CommandServiceNotRegisteredException(typeof(ISampleService));

            // assert
            Assert.Equal("Command service implementing Chel.UnitTests.Services.ISampleService has not been registered", sut.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsCommandServiceTypeProperty()
        {
            // arrange, act
            var sut = new CommandServiceNotRegisteredException(typeof(ISampleService));

            // assert
            Assert.Equal(typeof(ISampleService), sut.CommandServiceType);
        }
    }
}