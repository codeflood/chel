using System;
using Chel.UnitTests.Services;
using Xunit;

namespace Chel.UnitTests
{
    public class CommandServicesTests
    {
        [Fact]
        public void Register_ServiceIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandServices();
            Action sutAction = () => sut.Register<object>(null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("service", ex.ParamName);
        }

        [Fact]
        public void Register_ObjectInstanceProvided_DoesNotThrow()
        {
            // arrange
            var sut = new CommandServices();
            var service = new SampleService();

            // act
            sut.Register(service);
        }

        [Fact]
        public void Register_InterfaceTypeUsed_DoesNotThrow()
        {
            // arrange
            var sut = new CommandServices();
            var service = new SampleService();

            // act
            sut.Register<ISampleService>(service);
        }

        [Fact]
        public void Register_BaseInterfaceTypeUsed_DoesNotThrow()
        {
            // arrange
            var sut = new CommandServices();
            var service = new SampleService();

            // act
            sut.Register<ISuperSampleService>(service);
        }

        [Fact]
        public void Register_TypeAlreadyUsed_ThrowsException()
        {
            // arrange
            var sut = new CommandServices();
            var service1 = new SampleService();
            var service2 = new SampleService();
            sut.Register(service1);
            Action sutAction = () => sut.Register(service2);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("The service type Chel.UnitTests.Services.SampleService has already been registered", ex.Message);
        }

        [Fact]
        public void Register_ServiceAlreadyRegisteredUnderDifferentType_DoesNotThrow()
        {
            // arrange
            var sut = new CommandServices();
            var service = new SampleService();
            sut.Register<ISuperSampleService>(service);

            // act
            sut.Register<ISampleService>(service);
        }

        [Fact]
        public void ResolveGeneric_ServiceTypeNotRegistered_ReturnsNull()
        {
            // arrange
            var sut = new CommandServices();

            // act
            var service = sut.Resolve<ISampleService>();

            // assert
            Assert.Null(service);
        }

        [Fact]
        public void ResolveGeneric_ServiceIsRegistered_ReturnsService()
        {
            // arrange
            var sut = new CommandServices();
            var service = new SampleService();
            sut.Register(service);

            // act
            var resolvedService = sut.Resolve<SampleService>();

            // assert
            Assert.Same(service, resolvedService);
        }

        [Fact]
        public void ResolveGeneric_UseInterfaceType_ReturnsService()
        {
            // arrange
            var sut = new CommandServices();
            var service = new SampleService();
            sut.Register<ISampleService>(service);

            // act
            var resolvedService = sut.Resolve<ISampleService>();

            // assert
            Assert.Same(service, resolvedService);
        }

        [Fact]
        public void Resolve_ServiceTypeNotRegistered_ReturnsNull()
        {
            // arrange
            var sut = new CommandServices();

            // act
            var service = sut.Resolve(typeof(ISampleService));

            // assert
            Assert.Null(service);
        }

        [Fact]
        public void Resolve_ServiceIsRegistered_ReturnsService()
        {
            // arrange
            var sut = new CommandServices();
            var service = new SampleService();
            sut.Register(service);

            // act
            var resolvedService = sut.Resolve(typeof(SampleService));

            // assert
            Assert.Same(service, resolvedService);
        }

        [Fact]
        public void Resolve_UseInterfaceType_ReturnsService()
        {
            // arrange
            var sut = new CommandServices();
            var service = new SampleService();
            sut.Register<ISampleService>(service);

            // act
            var resolvedService = sut.Resolve(typeof(ISampleService));

            // assert
            Assert.Same(service, resolvedService);
        }
    }
}