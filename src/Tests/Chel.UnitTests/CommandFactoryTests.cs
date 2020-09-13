using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.UnitTests.SampleCommands;
using Chel.UnitTests.SampleObjects;
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
            var scopedObjects = new ScopedObjectRegistry();
            Action sutAction = () => new CommandFactory(null, services, scopedObjects);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandRegistry", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandServicesNull_ThrowsException()
        {
            // arrange
            var registry = Substitute.For<ICommandRegistry>();
            var scopedObjects = new ScopedObjectRegistry();
            Action sutAction = () => new CommandFactory(registry, null, scopedObjects);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandServices", ex.ParamName);
        }

        [Fact]
        public void Ctor_SessionObjectsNull_ThrowsException()
        {
            // arrange
            var registry = Substitute.For<ICommandRegistry>();
            var services = new CommandServices();
            Action sutAction = () => new CommandFactory(registry, services, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("sessionObjects", ex.ParamName);
        }

        [Fact]
        public void Create_InputIsNull_ThrowsException()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services, sessionObjects) => {});
            Action sutAction = () => sut.Create(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandInput", ex.ParamName);
        }

        [Fact]
        public void Create_CommandNotRegistered_ReturnsNull()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services, sessionObjects) => {});
            var input = CreateCommandInput(1, "command");

            // act
            var command = sut.Create(input);

            // assert
            Assert.Null(command);
        }

        [Fact]
        public void Create_CommandRegistered_ReturnsCommandInstance()
        {
            // arrange
            var sut = CreateCommandFactory((registry, service, sessionObjectss) =>
            {
                registry.Register(typeof(SampleCommand));
            });
            var input = CreateCommandInput(1, "sample");

            // act
            var command = sut.Create(input);

            // assert
            Assert.IsType<SampleCommand>(command);
        }

        [Fact]
        public void Create_MultipleCalls_ReturnsNewInstanceOnEachCall()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services, sessionObjects) =>
            {
                registry.Register(typeof(SampleCommand));
            });
            var input = CreateCommandInput(1, "sample");

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
            var sut = CreateCommandFactory((registry, services, sessionObjects) =>
            {
                registry.Register(typeof(ServiceDependencyCommand));
                services.Register<ISampleService>(new SampleService());
            });
            var input = CreateCommandInput(1, "alpha");

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
            var sut = CreateCommandFactory((registry, services, sessionObjects) =>
            {
                registry.Register(typeof(ServiceDependencyCommand));
            });
            var input = CreateCommandInput(1, "alpha");
            Action sutAction = () => sut.Create(input);

            // act, assert
            var ex = Assert.Throws<CommandDependencyNotRegisteredException>(sutAction);
            Assert.Equal(typeof(ISampleService), ex.CommandServiceType);
        }

        [Fact]
        public void Create_CommandRequiresRegisteredSessionObject_ReturnsCommandInstance()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services, sessionObjects) =>
            {
                registry.Register(typeof(SessionObjectCommand));
                sessionObjects.Register<GoodObject>();
            });
            var input = CreateCommandInput(1, "alpha");

            // act
            var command = sut.Create(input);
            var result = command.Execute();

            // assert
            Assert.IsType<SuccessResult>(result);
        }

        [Fact]
        public void Create_CommandRequiresServiceAndSessionObject_ReturnsCommandInstance()
        {
            // arrange
            var sut = CreateCommandFactory((registry, services, sessionObjects) =>
            {
                registry.Register(typeof(ServiceAndSessionObjectCommand));
                services.Register<ISampleService>(new SampleService());
                sessionObjects.Register<GoodObject>();
            });
            var input = CreateCommandInput(1, "alpha");

            // act
            var command = sut.Create(input);
            var result = command.Execute();

            // assert
            Assert.IsType<SuccessResult>(result);
        }

        private CommandFactory CreateCommandFactory(Action<CommandRegistry, CommandServices, ScopedObjectRegistry> configurator)
        {
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);
            var services = new CommandServices();
            var sessionObjects = new ScopedObjectRegistry();

            configurator.Invoke(registry, services, sessionObjects);

            return new CommandFactory(registry, services, sessionObjects);
        }

        private CommandInput CreateCommandInput(int sourceLine, string commandName)
        {
            var builder = new CommandInput.Builder(sourceLine, commandName);
            return builder.Build();
        }
    }
}