using System;
using Chel.Exceptions;
using Chel.UnitTests.SampleCommands;
using Xunit;

namespace Chel.UnitTests
{
    public class InvalidParameterDefinitionExceptionTests
    {
        [Fact]
        public void Ctor_PropertyIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new InvalidParameterDefinitionException(null, "");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("property", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var property = typeof(NumberedParameterCommand).GetProperty("NumberedParameter1");

            // act
            var sut = new InvalidParameterDefinitionException(property, "message");

            // assert
            Assert.Equal(property, sut.Property);
            Assert.Equal("message", sut.Message);
        }
    }
}