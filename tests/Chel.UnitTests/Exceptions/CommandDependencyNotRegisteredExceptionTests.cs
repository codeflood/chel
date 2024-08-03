using System;
using Chel.UnitTests.Services;
using Xunit;

namespace Chel.UnitTests.Exceptions
{
    public class CommandDependencyNotRegisteredExceptionTests
    {
        [Fact]
        public void Ctor_TypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDependencyNotRegisteredException(null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("type", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsMessageProperty()
        {
            // arrange, act
            var sut = new CommandDependencyNotRegisteredException(typeof(ISampleService));

            // assert
            Assert.Equal("Command dependency implementing Chel.UnitTests.Services.ISampleService has not been registered", sut.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsCommandServiceTypeProperty()
        {
            // arrange, act
            var sut = new CommandDependencyNotRegisteredException(typeof(ISampleService));

            // assert
            Assert.Equal(typeof(ISampleService), sut.CommandServiceType);
        }
    }
}