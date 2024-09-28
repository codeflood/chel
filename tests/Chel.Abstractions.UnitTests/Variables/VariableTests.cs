using System;
using Chel.Abstractions.Types;
using Chel.Abstractions.Variables;
using Xunit;

namespace Chel.Abstractions.UnitTests.Variables
{
    public class VariableTests
    {
        [Fact]
        public void Ctor_NameIsNull_ThrowsException()
        {
            // arrange
            var value = new Literal("value");
            Action sutAction = () => new Variable(null!, value);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void Ctor_NameIsEmpty_ThrowsException()
        {
            // arrange
            var value = new Literal("value");
            Action sutAction = () => new Variable("", value);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void Ctor_ValueIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Variable("name", null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var value = new Literal("value");

            // act
            var sut = new Variable("name", value);

            // assert
            Assert.Equal(value, sut.Value);
        }
    }
}