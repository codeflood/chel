using System;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandAttributeTests
    {
        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandAttribute(null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandAttribute("");

            // act, assert
            var ex = Assert.Throws<InvalidNameException>(sutAction);
            Assert.Equal("", ex.Name);
        }

        [Fact]
        public void Ctor_ModuleNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandAttribute("command", "");

            // act, assert
            var ex = Assert.Throws<InvalidNameException>(sutAction);
            Assert.Equal("", ex.Name);
        }

        [Fact]
        public void Ctor_CommandNameIsValid_PropertiesAreSet()
        {
            // arrange, act
            var sut = new CommandAttribute("command");

            // assert
            Assert.Equal("command", sut.CommandName);
        }

        [Fact]
        public void Ctor_ParametersAreValid_PropertiesAreSet()
        {
            // arrange, act
            var sut = new CommandAttribute("command", "module");

            // assert
            Assert.Equal("command", sut.CommandName);
            Assert.Equal("module", sut.ModuleName);
        }

        [Fact]
        public void CommandIdentifier_WhenCalled_ReturnsExecutionTargetIdentifier()
        {
            // arrange
            var sut = new CommandAttribute("command", "module");

            // act
            var commandIdentifier = sut.CommandIdentifier;

            // assert
            Assert.Equal("command", commandIdentifier.Name);
            Assert.Equal("module", commandIdentifier.Module);
        }
    }
}