using System;
using Xunit;

namespace Chel.UnitTests
{
    public class CommandRegistryTests
    {
        [Fact]
        public void Register_TypeIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandRegistry();
            Action sutAction = () => sut.Register(null);

            // act, assert
            Assert.Throws<ArgumentNullException>(sutAction);
        }

        [Fact]
        public void Register_TypeIsNotACommand_ThrowsException()
        {
            // arrange
            var sut = new CommandRegistry();
            Action sutAction = () => sut.Register(GetType());

            // act, assert
            Assert.Throws<ArgumentException>(sutAction);
        }

        [Fact]
        public void Register_TypeIsMissingCommandAttribute_ThrowsException()
        {
            // arrange
            var sut = new CommandRegistry();
            Action sutAction = () => sut.Register(typeof(BadSampleCommand));

            // act, assert
            Assert.Throws<ArgumentException>(sutAction);
        }
    }
}