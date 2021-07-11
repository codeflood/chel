using System;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandDescriptorTests
    {
        [Fact]
        public void GetDescription_CultureIsNull_ThrowsException()
        {
            // arrange
            var texts = Substitute.For<ITextResolver>();
            texts.GetText("en").Returns("text");

            var builder = new CommandDescriptor.Builder("command", GetType(), texts);
            var sut = builder.Build();
            Action sutAction = () => sut.GetDescription(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("cultureName", ex.ParamName);
        }

        [Fact]
        public void GetDescription_DescriptionHasBeenSet_ReturnsDescription()
        {
            // arrange
            var texts = Substitute.For<ITextResolver>();
            texts.GetText("en").Returns("text");

            var builder = new CommandDescriptor.Builder("command", GetType(), texts);
            var sut = builder.Build();

            // act
            var result = sut.GetDescription("en");

            // assert
            Assert.Equal("text", result);
        }
    }
}