using System;
using System.Collections.Generic;
using Chel.Abstractions.UnitTests.SampleCommands;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandDescriptorTests
    {
        [Fact]
        public void Ctor_ImplementingTypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor("command", null, Substitute.For<ITextResolver>(), new List<ParameterDescriptor>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("implementingType", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor(null, GetType(), Substitute.For<ITextResolver>(), new List<ParameterDescriptor>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor("", GetType(), Substitute.For<ITextResolver>(), new List<ParameterDescriptor>());

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
            Assert.Contains("commandName cannot be empty", ex.Message);
        }  

        [Fact]
        public void Ctor_TextResolverIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor("command", GetType(), null, new List<ParameterDescriptor>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("descriptions", ex.ParamName);
        }

        [Fact]
        public void Ctor_ParameterDescriptorsIsNull_NumberedParametersIsEmpty()
        {
            // arrange
            var texts = Substitute.For<ITextResolver>();

            // act
            var sut = new CommandDescriptor("command", GetType(), texts, null);

            // assert
            Assert.Empty(sut.NumberedParameters);
        }

        [Fact]
        public void Ctor_ValidParameters_PropertiesSet()
        {
            // arrange
            var type = typeof(SampleCommand);
            var commandTexts = Substitute.For<ITextResolver>();

            var property = type.GetProperty("Parameter");
            var parameterTexts = Substitute.For<ITextResolver>();
            var numberedParameters = new List<NumberedParameterDescriptor>
            {
                new NumberedParameterDescriptor(1, "num", property, parameterTexts)
            };

            // act
            var sut = new CommandDescriptor("command", type, commandTexts, numberedParameters);

            // assert
            Assert.Equal("command", sut.CommandName);
            Assert.Equal(type, sut.ImplementingType);
            
            var numberedParameter1 = sut.NumberedParameters[0];
            Assert.Equal(1, numberedParameter1.Number);
            Assert.Equal("num", numberedParameter1.PlaceholderText);
            Assert.Equal(property, numberedParameter1.Property);
        }

        [Fact]
        public void GetDescription_CultureIsNull_ThrowsException()
        {
            // arrange
            var texts = Substitute.For<ITextResolver>();
            texts.GetText("en").Returns("text");

            var sut = new CommandDescriptor("command", GetType(), texts, new List<ParameterDescriptor>());
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

            var sut = new CommandDescriptor("command", GetType(), texts, new List<ParameterDescriptor>());

            // act
            var result = sut.GetDescription("en");

            // assert
            Assert.Equal("text", result);
        }

    }
}