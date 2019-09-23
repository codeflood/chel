using System;
using Chel.UnitTests.SampleCommands;
using Xunit;

namespace Chel.UnitTests
{
    public class CommandRegistryTests
    {
        [Fact]
        public void Ctor_NullNameValidator_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandRegistry(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("nameValidator", ex.ParamName);
        }

        [Fact]
        public void Register_TypeIsNull_ThrowsException()
        {
            // arrange
            var sut = CreateCommandRegistry();
            Action sutAction = () => sut.Register(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("type", ex.ParamName);
        }

        [Fact]
        public void Register_TypeIsNotACommand_ThrowsException()
        {
            // arrange
            var sut = CreateCommandRegistry();
            Action sutAction = () => sut.Register(GetType());

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("type", ex.ParamName);
            Assert.Contains("CommandRegistryTests does not implement ICommand.", ex.Message);
        }

        [Fact]
        public void Register_TypeIsMissingCommandAttribute_ThrowsException()
        {
            // arrange
            var sut = CreateCommandRegistry();
            Action sutAction = () => sut.Register(typeof(MissingAttributeSampleCommand));

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("type", ex.ParamName);
            Assert.Contains("MissingAttributeSampleCommand is not attributed with CommandAttribute.", ex.Message);
        }

        [Fact]
        public void Register_CommandNameIsInvalid_ThrowsException()
        {
            // arrange
            var sut = CreateCommandRegistry();
            Action sutAction = () => sut.Register(typeof(InvalidCommandNameSampleCommand));

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("type", ex.ParamName);
            Assert.Contains("'  ' is not a valid command name.", ex.Message);
        }

        [Fact]
        public void Register_CommandAlreadyRegistered_DoesNotThrowException()
        {
            // arrange
            var sut = CreateCommandRegistry();
            sut.Register(typeof(SampleCommand));

            // act
            sut.Register(typeof(SampleCommand));
        }

        // different command, same command name.
        [Fact]
        public void Register_CommandNameAlreadyUsedOnDifferentCommand_ThrowsException()
        {
            // arrange
            var sut = CreateCommandRegistry();
            sut.Register(typeof(SampleCommand));
            Action sutAction = () => sut.Register(typeof(DuplicateSampleCommand));

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("type", ex.ParamName);
            Assert.Contains("Command name 'sample' on command DuplicateSampleCommand is already used on command type SampleCommand.", ex.Message);
        }

        [Fact]
        public void Resolve_NullCommandName_ThrowsException()
        {
            // arrange
            var sut = CreateCommandRegistry();
            Action sutAction = () => sut.Resolve(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Resolve_CommandNotRegistered_ReturnsNull()
        {
            // arrange
            var sut = CreateCommandRegistry();

            // act
            var command = sut.Resolve("command");

            // assert
            Assert.Null(command);
        }

        [Fact]
        public void Resolve_CommandIsRegistered_ReturnsCommandType()
        {
            // arrange
            var sut = CreateCommandRegistry();
            sut.Register(typeof(SampleCommand));

            // act
            var commandType = sut.Resolve("sample");

            // assert
            Assert.Equal(typeof(SampleCommand), commandType);
        }

        [Fact]
        public void Resolve_DifferentCasing_ReturnsCommandType()
        {
            // arrange
            var sut = CreateCommandRegistry();
            sut.Register(typeof(SampleCommand));

            // act
            var commandType = sut.Resolve("SAmPLE");

            // assert
            Assert.Equal(typeof(SampleCommand), commandType);
        }

        private CommandRegistry CreateCommandRegistry()
        {
            var nameValidator = new NameValidator();
            return new CommandRegistry(nameValidator);
        }
    }
}