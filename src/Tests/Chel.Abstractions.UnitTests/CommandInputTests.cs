using System;
using Chel.Abstractions;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandInputTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Ctor_SourceLineIsNonPositive_ThrowsException(int sourceLine)
        {
            // arrange
            Action sutAction = () => new CommandInput(sourceLine, "command");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("sourceLine", ex.ParamName);
            Assert.Contains("sourceLine must be greater than 0", ex.Message);
        }

        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandInput(1, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameProvided_PropertySet()
        {
            // arrange, act
            var sut = new CommandInput(3, "command");

            // assert
            Assert.Equal(3, sut.SourceLine);
            Assert.Equal("command", sut.CommandName);
        }

        [Fact]
        public void Equals_CommandsAreSame_ReturnsTrue()
        {
            // arrange
            var sut1 = new CommandInput(2, "command");
            var sut2 = new CommandInput(2, "command");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "command", 2, "command")]
        [InlineData(1, "command", 1, "other")]
        public void Equals_CommandsAreDifferent_ReturnsFalse(int sourceLine1, string commandName1, int sourceLine2, string commandName2)
        {
            // arrange
            var sut1 = new CommandInput(sourceLine1, commandName1);
            var sut2 = new CommandInput(sourceLine2, commandName2);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_CommandNamesCasingDifferent_ReturnsTrue()
        {
            // arrange
            var sut1 = new CommandInput(4, "command");
            var sut2 = new CommandInput(4, "COMMAND");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void GetHashCode_CommandsAreSame_HashCodeAreSame()
        {
            // arrange
            var sut1 = new CommandInput(2, "command");
            var sut2 = new CommandInput(2, "command");

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashcode1, hashcode2);
        }

        [Theory]
        [InlineData(1, "command", 2, "command")]
        [InlineData(1, "command", 1, "other")]
        public void GetHashCode_CommandsAreDifferent_HashCodeAreDifferent(int sourceLine1, string commandName1, int sourceLine2, string commandName2)
        {
            // arrange
            var sut1 = new CommandInput(sourceLine1, commandName1);
            var sut2 = new CommandInput(sourceLine2, commandName2);

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.NotEqual(hashcode1, hashcode2);
        }

        [Fact]
        public void GetHashCode_CommandNamesCasingDifferent_HashCodeAreSame()
        {
            // arrange
            var sut1 = new CommandInput(1, "command");
            var sut2 = new CommandInput(1, "COMmAND");

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashcode1, hashcode2);
        }
    }
}
