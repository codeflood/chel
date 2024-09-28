using System;
using System.Reflection;
using Chel.Abstractions.UnitTests.SampleCommands;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class ParameterDescriptorTests
    {
        [Fact]
        public void Ctor_PropertyIsNull_ThrowsException()
        {
            // arrange
            var textResolver = Substitute.For<ITextResolver>();
            Action sutAction = () => Substitute.ForPartsOf<ParameterDescriptor>(null, textResolver, false);

            // act, assert
            var ex = Assert.Throws<TargetInvocationException>(sutAction);
            var innerEx = ex.InnerException as ArgumentNullException;
            Assert.Equal("property", innerEx!.ParamName);
        }

        [Fact]
        public void Ctor_DescriptionsIsNull_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            Action sutAction = () => Substitute.ForPartsOf<ParameterDescriptor>(property, null, false);

            // act, assert
            var ex = Assert.Throws<TargetInvocationException>(sutAction);
            var innerEx = ex.InnerException as ArgumentNullException;
            Assert.Equal("descriptions", innerEx!.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var property = CreateProperty();
            var textResolver = Substitute.For<ITextResolver>();

            // act
            var sut = Substitute.ForPartsOf<ParameterDescriptor>(property, textResolver, true);

            // assert
            Assert.Equal(property, sut.Property.Property);
            Assert.True(sut.Required);
        }

        [Fact]
        public void GetDescription_CultureIsNull_ThrowsException()
        {
            // arrange
            var property = CreateProperty();
            var texts = Substitute.For<ITextResolver>();
            texts.GetText("en").Returns("text");

            var sut = Substitute.ForPartsOf<ParameterDescriptor>(property, texts, false);
            Action sutAction = () => sut.GetDescription(null!);

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

            var sut = Substitute.ForPartsOf<ParameterDescriptor>(property, texts, false);

            // act
            var result = sut.GetDescription("en");

            // assert
            Assert.Equal("text", result);
        }

        private PropertyInfo CreateProperty()
        {
            return typeof(SampleCommand).GetProperty("Parameter")!;
        }
    }
}