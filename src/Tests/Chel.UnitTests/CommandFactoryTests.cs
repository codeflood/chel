using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.UnitTests.SampleCommands;
using Chel.UnitTests.Services;
using NSubstitute;
using Xunit;

namespace Chel.UnitTests
{
    public class CommandFactoryTests
    {
        [Fact]
        public void Ctor_CommandRegistryNull_ThrowsException()
        {
            // arrange
            var services = new CommandServices();
            Action sutAction = () => new CommandFactory(null, services);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandRegistry", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandServicesNull_ThrowsException()
        {
            // arrange
            var registry = Substitute.For<ICommandRegistry>();
            Action sutAction = () => new CommandFactory(registry, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandServices", ex.ParamName);
        }

        [Fact]
        public void Create_InputIsNull_ThrowsException()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services) => {});
            Action sutAction = () => sut.Create(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandInput", ex.ParamName);
        }

        [Fact]
        public void Create_CommandNotRegistered_ReturnsNull()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services) => {});
            var input = new CommandInput(1, "command");

            // act
            var command = sut.Create(input);

            // assert
            Assert.Null(command);
        }

        [Fact]
        public void Create_CommandRegistered_ReturnsCommandInstance()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services) =>
            {
                registry.Register(typeof(SampleCommand));
            });
            var input = new CommandInput(1, "sample");

            // act
            var command = sut.Create(input);

            // assert
            Assert.IsType<SampleCommand>(command);
        }

        [Fact]
        public void Create_MultipleCalls_ReturnsNewInstanceOnEachCall()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services) =>
            {
                registry.Register(typeof(SampleCommand));
            });
            var input = new CommandInput(1, "sample");

            // act
            var command = sut.Create(input);
            var command2 = sut.Create(input);

            // assert
            Assert.NotSame(command, command2);
        }

        [Fact]
        public void Create_CommandRequiresRegisteredService_ReturnsCommandInstance()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services) =>
            {
                registry.Register(typeof(ServiceDependencyCommand));
                services.Register<ISampleService>(new SampleService());
            });
            var input = new CommandInput(1, "alpha");

            // act
            var command = sut.Create(input);
            var result = command.Execute();

            // assert
            Assert.IsType<SuccessResult>(result);
        }

        [Fact]
        public void Create_CommandRequiresNotRegisteredService_ThrowsException()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services) =>
            {
                registry.Register(typeof(ServiceDependencyCommand));
            });
            var input = new CommandInput(1, "alpha");
            Action sutAction = () => sut.Create(input);

            // act, assert
            var ex = Assert.Throws<CommandServiceNotRegisteredException>(sutAction);
            Assert.Equal(typeof(ISampleService), ex.CommandServiceType);
        }

        private CommandFactory CreateCommandFactory(Action<CommandRegistry, CommandServices> configurator)
        {
            var nameValidator = new NameValidator();
            var registry = new CommandRegistry(nameValidator);
            var services = new CommandServices();

            configurator.Invoke(registry, services);

            return new CommandFactory(registry, services);
        }
    }
}