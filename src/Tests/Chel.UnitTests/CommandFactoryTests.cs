using System;
using Chel.Abstractions;
using Chel.UnitTests.SampleCommands;
using Xunit;

namespace Chel.UnitTests
{
    public class CommandFactoryTests
    {
        [Fact]
        public void Ctor_CommandRegistryNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandFactory(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandRegistry", ex.ParamName);
        }

        [Fact]
        public void Create_InputIsNull_ThrowsException()
        {
            // arrange
            var sut = CreateCommandFactory();
            Action sutAction = () => sut.Create(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandInput", ex.ParamName);
        }

        [Fact]
        public void Create_CommandNotRegistered_ReturnsNull()
        {
            // arrange
            var sut = CreateCommandFactory();
            var input = new CommandInput("command");

            // act
            var command = sut.Create(input);

            // assert
            Assert.Null(command);
        }

        [Fact]
        public void Create_CommandRegistered_ReturnsCommandInstance()
        {
            // arrange
            var sut = CreateCommandFactory(typeof(SampleCommand));
            var input = new CommandInput("sample");

            // act
            var command = sut.Create(input);

            // assert
            Assert.IsType<SampleCommand>(command);
        }

        [Fact]
        public void Create_MultipleCalls_ReturnsNewInstanceOnEachCall()
        {
            // arrange
            var sut = CreateCommandFactory(typeof(SampleCommand));
            var input = new CommandInput("sample");

            // act
            var command = sut.Create(input);
            var command2 = sut.Create(input);

            // assert
            Assert.NotSame(command, command2);
        }

        private CommandFactory CreateCommandFactory(params Type[] commandTypes)
        {
            var nameValidator = new NameValidator();
            var registry = new CommandRegistry(nameValidator);

            foreach(var type in commandTypes)
                registry.Register(type);

            return new CommandFactory(registry);
        }
    }
}