using System;
using Chel.UnitTests.SampleCommands;
using Xunit;

namespace Chel.UnitTests.Exceptions
{
    public class CommandNameAlreadyUsedExceptionTests
    {
        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandNameAlreadyUsedException(null, typeof(SampleCommand), typeof(SampleCommand));

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandTypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandNameAlreadyUsedException("command", null, typeof(SampleCommand));

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandType", ex.ParamName);
        }

        [Fact]
        public void Ctor_OtherCommandTypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandNameAlreadyUsedException("command", typeof(SampleCommand), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("otherCommandType", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var otherCommandType = typeof(SampleCommand);
            var commandType = typeof(DuplicateSampleCommand);
            var sut = new CommandNameAlreadyUsedException("sample", commandType, otherCommandType);

            // assert
            Assert.Equal("sample", sut.CommandName);
            Assert.Equal(otherCommandType, sut.OtherCommandType);
            Assert.Equal(commandType, sut.CommandType);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsMessage()
        {
            // arrange
            var otherCommandType = typeof(SampleCommand);
            var commandType = typeof(DuplicateSampleCommand);
            var sut = new CommandNameAlreadyUsedException("sample", commandType, otherCommandType);

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("Chel.CommandNameAlreadyUsedException: Command name 'sample' on command type Chel.UnitTests.SampleCommands.DuplicateSampleCommand is already used on command type Chel.UnitTests.SampleCommands.SampleCommand", result);
        }
    }
}