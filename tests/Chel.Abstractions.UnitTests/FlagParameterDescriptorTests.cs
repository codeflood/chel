using System;
using System.Reflection;
using Chel.Abstractions.UnitTests.SampleCommands;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class FlagParameterDescriptorTests
    {
        [Fact]
        public void Ctor_NameIsNull_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new FlagParameterDescriptor(null!, property, textResolver, false);

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
            Action sutAction = () => new FlagParameterDescriptor(name, property, textResolver, false);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
            Assert.Contains("'name' cannot be empty or whitespace", ex.Message);
        }

        [Fact]
        public void Ctor_RequiredIsTrue_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => new FlagParameterDescriptor("name", property, textResolver, true);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("required", ex.ParamName);
            Assert.Contains("Flag parameters cannot be marked as required (name)", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();

            // act
            var sut = new FlagParameterDescriptor("name", property, textResolver, false);

            // assert
            Assert.Equal("name", sut.Name);
            Assert.Equal(property, sut.Property.Property);
            Assert.False(sut.Required);
        }

        private PropertyInfo CreateProperty()
        {
            return typeof(SampleCommand).GetProperty("Parameter")!;
        }
    }
}