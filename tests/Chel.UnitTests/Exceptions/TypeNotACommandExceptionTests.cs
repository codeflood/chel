using System;
using Chel.Exceptions;
using Chel.UnitTests.SampleCommands;
using Xunit;

namespace Chel.UnitTests.Exceptions
{
    public class TypeNotACommandExceptionTests
    {
        [Fact]
        public void Ctor_TypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new TypeNotACommandException(null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("type", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsMessageProperty()
        {
            // arrange, act
            var sut = new TypeNotACommandException(typeof(MissingAttributeSampleCommand));

            // assert
            Assert.Equal("Chel.UnitTests.SampleCommands.MissingAttributeSampleCommand is not a command", sut.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsCommandServiceTypeProperty()
        {
            // arrange, act
            var sut = new TypeNotACommandException(typeof(MissingAttributeSampleCommand));

            // assert
            Assert.Equal(typeof(MissingAttributeSampleCommand), sut.Type);
        }
    }
}