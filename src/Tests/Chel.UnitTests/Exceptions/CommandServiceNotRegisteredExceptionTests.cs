using Chel.UnitTests.Services;
using Xunit;

namespace Chel.UnitTests.Exceptions
{
    public class CommandServiceNotRegisteredExceptionTests
    {
        [Fact]
        public void Ctor_WhenCalled_SetsMessageProperty()
        {
            // arrange, act
            var sut = new CommandServiceNotRegisteredException(typeof(ISampleService));

            // assert
            Assert.Equal("Command service implementing ISampleService has not been registered", sut.Message);
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