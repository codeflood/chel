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
            Action sutAction = () => new CommandAttribute(null);

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
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsValid_PropertyIsSet()
        {
            // arrange, act
            var sut = new CommandAttribute("command");

            // assert
            Assert.Equal("command", sut.CommandName);
        }
    }
}