using System;
using System.Globalization;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class DescriptionAttributeTests
    {
        [Fact]
        public void Ctor_TextIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new DescriptionAttribute(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("text", ex.ParamName);
        }

        [Fact]
        public void Ctor_TextIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new DescriptionAttribute("");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("text", ex.ParamName);
            Assert.Contains("'text' cannot be empty", ex.Message);
        }

        [Fact]
        public void Ctor_CultureNameIsNull_InvariantCultureSetOnProperty()
        {
            // act
            var sut = new DescriptionAttribute("description", null);

            // assert
            Assert.Equal(CultureInfo.InvariantCulture.Name, sut.CultureName);
        }

        [Fact]
        public void Ctor_NoCultureProvided_InvariantCultureSetOnProperty()
        {
            // act
            var sut = new DescriptionAttribute("description");

            // assert
            Assert.Equal(CultureInfo.InvariantCulture.Name, sut.CultureName);
        }

        [Fact]
        public void Ctor_WhenCalled_PropertiesAreSet()
        {
            // act
            var sut = new DescriptionAttribute("description", "en-AU");

            // assert
            Assert.Equal("description", sut.Text);
            Assert.Equal("en-AU", sut.CultureName);
        }
    }
}