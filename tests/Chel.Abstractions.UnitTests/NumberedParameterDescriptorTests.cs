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
            Action sutAction = () => new NumberedParameterDescriptor(number, "param", property, textResolver, false);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("number", ex.ParamName);
            Assert.Contains("'number' must be greater than 0", ex.Message);
        }

        [Fact]
        public void Ctor_PlaceholderTextIsNull_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new NumberedParameterDescriptor(1, null, property, textResolver, false);

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
            Action sutAction = () => new NumberedParameterDescriptor(1, "", property, textResolver, false);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("placeholderText", ex.ParamName);
            Assert.Contains("'placeholderText' cannot be empty", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();

            // act
            var sut = new NumberedParameterDescriptor(1, "param", property, textResolver, true);

            // assert
            Assert.Equal(1, sut.Number);
            Assert.Equal("param", sut.PlaceholderText);
            Assert.Equal(property, sut.Property.Property);
            Assert.True(sut.Required);
        }

        private PropertyInfo CreateProperty()
        {
            return typeof(SampleCommand).GetProperty("Parameter");
        }
    }
}