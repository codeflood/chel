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

        private CommandRegistry CreateCommandRegistry()
        {
            var nameValidator = new NameValidator();
            return new CommandRegistry(nameValidator);
        }
    }
}