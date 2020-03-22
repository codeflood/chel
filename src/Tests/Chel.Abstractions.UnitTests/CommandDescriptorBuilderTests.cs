using System;
using System.Reflection;
using Chel.Abstractions.UnitTests.SampleCommands;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandDescriptorBuilderTests
    {
        [Fact]
        public void Ctor_ImplementingTypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder("command", null, Substitute.For<ITextResolver>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("implementingType", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(null, GetType(), Substitute.For<ITextResolver>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder("", GetType(), Substitute.For<ITextResolver>());

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
            Assert.Contains("commandName cannot be empty", ex.Message);
        }  

        [Fact]
        public void Ctor_TextResolverIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder("command", GetType(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("descriptions", ex.ParamName);
        }

        [Fact]
        public void AddNumberedParameter_DescriptorIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder("command", GetType(), Substitute.For<ITextResolver>());
            Action sutAction = () => sut.AddNumberedParameter(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("descriptor", ex.ParamName);
        }

        [Fact]
        public void AddNumberedParameter_DescriptorAlreadyAdded_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder("command", GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor1 = new NumberedParameterDescriptor(1, "ph", property, Substitute.For<ITextResolver>(), false);
            var descriptor2 = new NumberedParameterDescriptor(1, "ph", property, Substitute.For<ITextResolver>(), false);
            sut.AddNumberedParameter(descriptor1);
            Action sutAction = () => sut.AddNumberedParameter(descriptor2);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Descriptor has already been added.", ex.Message);
        }

        [Fact]
        public void AddNamedParameter_DescriptorIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder("command", GetType(), Substitute.For<ITextResolver>());
            Action sutAction = () => sut.AddNamedParameter(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("descriptor", ex.ParamName);
        }

        [Fact]
        public void AddNamedParameter_DescriptorAlreadyAdded_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder("command", GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor1 = new NamedParameterDescriptor("name", "value", property, Substitute.For<ITextResolver>(), false);
            var descriptor2 = new NamedParameterDescriptor("name", "value", property, Substitute.For<ITextResolver>(), false);
            sut.AddNamedParameter(descriptor1);
            Action sutAction = () => sut.AddNamedParameter(descriptor2);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Descriptor has already been added.", ex.Message);
        }

        [Fact]
        public void AddNamedParameter_DescriptorAlreadyAddedWithDifferentCasing_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder("command", GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor1 = new NamedParameterDescriptor("name", "value", property, Substitute.For<ITextResolver>(), false);
            var descriptor2 = new NamedParameterDescriptor("NAME", "value", property, Substitute.For<ITextResolver>(), false);
            sut.AddNamedParameter(descriptor1);
            Action sutAction = () => sut.AddNamedParameter(descriptor2);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Descriptor has already been added.", ex.Message);
        }

        [Fact]
        public void Build_WhenCalled_ReturnsCommandDescriptor()
        {
            // arrange
            var sut = new CommandDescriptor.Builder("command", GetType(), Substitute.For<ITextResolver>());

            // act
            var commandDescriptor = sut.Build();

            // assert
            Assert.Equal("command", commandDescriptor.CommandName);
            Assert.Equal(commandDescriptor.ImplementingType, GetType());
            Assert.Empty(commandDescriptor.NumberedParameters);
            Assert.Empty(commandDescriptor.NamedParameters);
        }

        [Fact]
        public void Build_AfterNumberedParameterAdded_ReturnsCommandDescriptorWithNumberedParameter()
        {
            // arrange
            var sut = new CommandDescriptor.Builder("command", GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor = new NumberedParameterDescriptor(1, "ph", property, Substitute.For<ITextResolver>(), false);
            sut.AddNumberedParameter(descriptor);

            // act
            var commandDescriptor = sut.Build();

            // assert
            Assert.Equal(1, commandDescriptor.NumberedParameters.Count);
            Assert.Equal(descriptor, commandDescriptor.NumberedParameters[0]);
        }

        [Fact]
        public void Build_AfterNamedParameterAdded_ReturnsCommandDescriptorWithNamedParameter()
        {
            // arrange
            var sut = new CommandDescriptor.Builder("command", GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor = new NamedParameterDescriptor("name", "value", property, Substitute.For<ITextResolver>(), false);
            sut.AddNamedParameter(descriptor);

            // act
            var commandDescriptor = sut.Build();

            // assert
            Assert.Equal(1, commandDescriptor.NamedParameters.Count);
            Assert.Equal(descriptor, commandDescriptor.NamedParameters["name"]);
        }

        private PropertyInfo CreateProperty()
        {
            return typeof(SampleCommand).GetProperty("Parameter");
        }
    }
}