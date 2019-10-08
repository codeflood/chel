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
            Action sutAction = () => new CommandAttribute(null, "lorem ipsum");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_DescriptionIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandAttribute("command", null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("description", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandAttribute("", "lorem ipsum");

            // act, assert
            var ex = Assert.Throws<InvalidCommandNameException>(sutAction);
            Assert.Equal("", ex.CommandName);
        }

        [Fact]
        public void Ctor_DescriptionIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandAttribute("command", "");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("description", ex.ParamName);
            Assert.Contains("description cannot be empty", ex.Message);
        }

        [Fact]
        public void Ctor_ParametersAreValid_PropertiesAreSet()
        {
            // arrange, act
            var sut = new CommandAttribute("command", "lorem ipsum");

            // assert
            Assert.Equal("command", sut.CommandName);
            Assert.Equal("lorem ipsum", sut.Description);
        }
    }
}