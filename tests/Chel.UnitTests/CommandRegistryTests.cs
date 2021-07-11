using System;
using Chel.Abstractions;
using Chel.Exceptions;
using Chel.UnitTests.Comparers;
using Chel.UnitTests.SampleCommands;
using NSubstitute;
using Xunit;

namespace Chel.UnitTests
{
    public class CommandRegistryTests
    {
        [Fact]
        public void Ctor_NullNameValidator_ThrowsException()
        {
            // arrange
            var descriptorGenerator = new CommandAttributeInspector();
            Action sutAction = () => new CommandRegistry(null, descriptorGenerator);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("nameValidator", ex.ParamName);
        }

        [Fact]
        public void Ctor_NullDescriptorGenerator_ThrowsException()
        {
            // arrange
            var nameValidator = new NameValidator();
            Action sutAction = () => new CommandRegistry(nameValidator, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandDescriptorGenerator", ex.ParamName);
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
            Assert.Contains("Chel.UnitTests.CommandRegistryTests does not implement ICommand", ex.Message);
        }

        [Fact]
        public void Register_TypeCannotBeDescribed_ThrowsException()
        {
            // arrange
            var nameValidator = new NameValidator();
            var descriptorGenerator = Substitute.For<ICommandDescriptorGenerator>();
            var sut = new CommandRegistry(nameValidator, descriptorGenerator);
            Action sutAction = () => sut.Register(typeof(SampleCommand));

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Descriptor for Chel.UnitTests.SampleCommands.SampleCommand could not be generated", ex.Message);
        }

        [Fact]
        public void Register_TypeIsMissingCommandAttribute_ThrowsException()
        {
            // arrange
            var sut = CreateCommandRegistry();
            Action sutAction = () => sut.Register(typeof(MissingAttributeSampleCommand));

            // act, assert
            var ex = Assert.Throws<TypeNotACommandException>(sutAction);
            Assert.Equal(typeof(MissingAttributeSampleCommand), ex.Type);
        }

        [Fact]
        public void Register_CommandNameIsInvalid_ThrowsException()
        {
            // arrange
            var sut = CreateCommandRegistry();
            Action sutAction = () => sut.Register(typeof(InvalidCommandNameSampleCommand));

            // act, assert
            var ex = Assert.Throws<InvalidCommandNameException>(sutAction);
            Assert.Equal("  ", ex.CommandName);
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

        [Fact]
        public void Register_CommandNameAlreadyUsedOnDifferentCommand_ThrowsException()
        {
            // arrange
            var sut = CreateCommandRegistry();
            sut.Register(typeof(SampleCommand));
            Action sutAction = () => sut.Register(typeof(DuplicateSampleCommand));

            // act, assert
            var ex = Assert.Throws<CommandNameAlreadyUsedException>(sutAction);
            Assert.Equal("sample", ex.CommandName);
            Assert.Equal(typeof(DuplicateSampleCommand), ex.CommandType);
            Assert.Equal(typeof(SampleCommand), ex.OtherCommandType);
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
        public void Resolve_CommandIsRegistered_ReturnsCommandDescriptor()
        {
            // arrange
            var sut = CreateCommandRegistry();
            sut.Register(typeof(SampleCommand));
            var descriptor = CreateSampleCommandDescriptor();

            // act
            var resolvedDescriptor = sut.Resolve("sample");

            // assert
            Assert.Equal(descriptor, resolvedDescriptor, new CommandDescriptorEqualityComparer());
        }

        [Fact]
        public void Resolve_DifferentCasing_ReturnsCommandDescriptor()
        {
            // arrange
            var sut = CreateCommandRegistry();
            sut.Register(typeof(SampleCommand));
            var descriptor = CreateSampleCommandDescriptor();

            // act
            var resolvedDescriptor = sut.Resolve("SAmPLE");

            // assert
            Assert.Equal(descriptor, resolvedDescriptor, new CommandDescriptorEqualityComparer());
        }

        [Fact]
        public void GetAllRegistrations_WhenNothingRegistered_ReturnsEmptyCollection()
        {
            // arrange
            var sut = CreateCommandRegistry();

            // act
            var types = sut.GetAllRegistrations();

            // assert
            Assert.Empty(types);
        }

        [Fact]
        public void GetAllRegistrations_WhenTypesRegistered_ReturnsDescriptors()
        {
            // arrange
            var sut = CreateCommandRegistry();
            sut.Register(typeof(SampleCommand));
            sut.Register(typeof(SampleCommand2));
            var descriptor1 = CreateSampleCommandDescriptor();
            var descriptor2 = CreateSampleCommand2Descriptor();

            // act
            var resolvedDescriptors = sut.GetAllRegistrations();

            // assert
            Assert.Equal(new[]{ descriptor1, descriptor2 }, resolvedDescriptors, new CommandDescriptorEqualityComparer());
        }

        private CommandRegistry CreateCommandRegistry()
        {
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            return new CommandRegistry(nameValidator, descriptorGenerator);
        }

        private CommandDescriptor CreateSampleCommandDescriptor()
        {
            var descriptions = new LocalisedTexts();
            descriptions.AddText("description", "en");

            var builder =  new CommandDescriptor.Builder("sample", typeof(SampleCommand), descriptions);
            return builder.Build();
        }

        private CommandDescriptor CreateSampleCommand2Descriptor()
        {
            var descriptions = new LocalisedTexts();
            descriptions.AddText("description", "en");

            var builder = new CommandDescriptor.Builder("sample2", typeof(SampleCommand2), descriptions);
            return builder.Build();
        }
    }
}