using System;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class FlagParameterAttributeTests
    {
        [Fact]
        public void Ctor_NameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new FlagParameterAttribute(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData(" ")]
        public void Ctor_NameIsEmptyOrWhitespace_ThrowsException(string name)
        {
            // arrange
            Action sutAction = () => new FlagParameterAttribute(name);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
            Assert.Contains("'name' cannot be empty or whitespace", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new FlagParameterAttribute("name");

            // assert
            Assert.Equal("name", sut.Name);
        }
    }
}