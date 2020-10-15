using System;
using Chel.Abstractions.Variables;
using Xunit;

namespace Chel.Abstractions.UnitTests.Variables
{
    public class ValueVariableTests
    {
        [Fact]
        public void Ctor_NameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new ValueVariable(null, "value");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void Ctor_NameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new ValueVariable("", "value");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void Ctor_ValueIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new ValueVariable("name", null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void Ctor_ValueIsEmpty_DoesNotThrow()
        {
            new ValueVariable("name", "");
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new ValueVariable("name", "value");

            // assert
            Assert.Equal("value", sut.Value);
        }
    }
}