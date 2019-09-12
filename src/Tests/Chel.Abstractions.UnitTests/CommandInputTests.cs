using System;
using Chel.Abstractions;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandInputTests
    {
        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandInput(null);

            // act, assert
            Assert.Throws<ArgumentException>(sutAction);
        }

        [Fact]
        public void Ctor_CommandNameProvided_PropertySet()
        {
            // arrange, act
            var sut = new CommandInput("command");

            // assert
            Assert.Equal("command", sut.CommandName);
        }

        [Fact]
        public void Equals_CommandNamesSame_ReturnsTrue()
        {
            // arrange
            var sut1 = new CommandInput("command");
            var sut2 = new CommandInput("command");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_CommandNamesCasingDifferent_ReturnsTrue()
        {
            // arrange
            var sut1 = new CommandInput("command");
            var sut2 = new CommandInput("COMMAND");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void GetHashCode_CommandNamesSame_HashCodeAreSame()
        {
            // arrange
            var sut1 = new CommandInput("command");
            var sut2 = new CommandInput("command");

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashcode1, hashcode2);
        }
    }
}
