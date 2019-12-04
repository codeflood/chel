using System;
using System.Reflection;
using Chel.Abstractions.UnitTests.SampleCommands;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class NumberedParameterDescriptorTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Ctor_NumberIsZeroOrLess_ThrowsException(int number)
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new NumberedParameterDescriptor(number, "param", property, textResolver);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("number", ex.ParamName);
            Assert.Contains("number must be greater than 0", ex.Message);
        }

        [Fact]
        public void Ctor_PlaceholderTextIsNull_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new NumberedParameterDescriptor(1, null, property, textResolver);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("placeholderText", ex.ParamName);
        }

        [Fact]
        public void Ctor_PlaceholderTextIsEmpty_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new NumberedParameterDescriptor(1, "", property, textResolver);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("placeholderText", ex.ParamName);
            Assert.Contains("placeholderText cannot be empty", ex.Message);
        }

        [Fact]
        public void Ctor_PropertyIsNull_ThrowsException()
        {
            // arrange
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new NumberedParameterDescriptor(1, "param", null, textResolver);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("property", ex.ParamName);
        }

        [Fact]
        public void Ctor_DescriptionsIsNull_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            Action sutAction = () => new NumberedParameterDescriptor(1, "param", property, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("descriptions", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();

            // act
            var sut = new NumberedParameterDescriptor(1, "param", property, textResolver);

            // assert
            Assert.Equal(1, sut.Number);
            Assert.Equal("param", sut.PlaceholderText);
            Assert.Equal(property, sut.Property);
        }

        [Fact]
        public void GetDescription_CultureIsNull_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            var texts = Substitute.For<ITextResolver>();
            texts.GetText("en").Returns("text");

            var sut = new NumberedParameterDescriptor(1, "param", property, texts);
            Action sutAction = () => sut.GetDescription(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("cultureName", ex.ParamName);
        }

        [Fact]
        public void GetDescription_DescriptionHasBeenSet_ReturnsDescription()
        {
            // arrange
            var property = CreateProperty();
            var texts = Substitute.For<ITextResolver>();
            texts.GetText("en").Returns("text");

            var sut = new NumberedParameterDescriptor(1, "param", property, texts);

            // act
            var result = sut.GetDescription("en");

            // assert
            Assert.Equal("text", result);
        }

        private PropertyInfo CreateProperty()
        {
            return typeof(SampleCommand).GetProperty("Parameter");
        }
    }
}