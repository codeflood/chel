using System;
using System.Reflection;
using Chel.Abstractions.UnitTests.SampleCommands;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class NamedParameterDescriptorTests
    {
        [Fact]
        public void Ctor_NameIsNull_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new NamedParameterDescriptor(null, "value", property, textResolver, false);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("\r\n")]
        [InlineData(" ")]
        public void Ctor_NameIsWhiteSpace_ThrowsException(string name)
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new NamedParameterDescriptor(name, "value", property, textResolver, false);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
            Assert.Contains("'name' cannot be empty or whitespace", ex.Message);
        }

        [Fact]
        public void Ctor_PlaceholderIsNull_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new NamedParameterDescriptor("name", null, property, textResolver, false);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("valuePlaceholderText", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("\r\n")]
        [InlineData(" ")]
        public void Ctor_PlaceholderIsWhiteSpace_ThrowsException(string placeholder)
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new NamedParameterDescriptor("name", placeholder, property, textResolver, false);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("valuePlaceholderText", ex.ParamName);
            Assert.Contains("'valuePlaceholderText' cannot be empty or whitespace", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();

            // act
            var sut = new NamedParameterDescriptor("name", "value", property, textResolver, true);

            // assert
            Assert.Equal("name", sut.Name);
            Assert.Equal("value", sut.ValuePlaceholderText);
            Assert.Equal(property, sut.Property.Property);
            Assert.True(sut.Required);
        }

        private PropertyInfo CreateProperty()
        {
            return typeof(SampleCommand).GetProperty("Parameter");
        }
    }
}