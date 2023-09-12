using System;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class NamedParameterAttributeTests
    {
        [Fact]
        public void Ctor_NameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new NamedParameterAttribute(null, "value");

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
            Action sutAction = () => new NamedParameterAttribute(name, "value");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
            Assert.Contains("'name' cannot be empty or whitespace", ex.Message);
        }

        [Fact]
        public void Ctor_PlaceholderIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new NamedParameterAttribute("name", null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("valuePlaceholderText", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData(" ")]
        public void Ctor_PlaceholderIsEmptyOrWhitespace_ThrowsException(string placeholder)
        {
            // arrange
            Action sutAction = () => new NamedParameterAttribute("name", placeholder);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("valuePlaceholderText", ex.ParamName);
            Assert.Contains("'valuePlaceholderText' cannot be empty or whitespace", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new NamedParameterAttribute("name", "value");

            // assert
            Assert.Equal("name", sut.Name);
            Assert.Equal("value", sut.ValuePlaceholderText);
        }
    }
}
