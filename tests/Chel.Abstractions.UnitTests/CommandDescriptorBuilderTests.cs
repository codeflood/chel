using System;
using System.Reflection;
using Chel.Abstractions.UnitTests.SampleCommands;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandDescriptorBuilderTests
    {
        private readonly ExecutionTargetIdentifier SampleCommandIdentifier = new ExecutionTargetIdentifier(null, "command");

        [Fact]
        public void Ctor_ImplementingTypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(SampleCommandIdentifier, null, Substitute.For<ITextResolver>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("implementingType", ex.ParamName);
        }

        [Fact]
        public void Ctor_TextResolverIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("descriptions", ex.ParamName);
        }

        [Fact]
        public void AddNumberedParameter_DescriptorIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
            Action sutAction = () => sut.AddNumberedParameter(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("descriptor", ex.ParamName);
        }

        [Fact]
        public void AddNumberedParameter_DescriptorAlreadyAdded_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor1 = new NumberedParameterDescriptor(1, "ph", property, Substitute.For<ITextResolver>(), false);
            var descriptor2 = new NumberedParameterDescriptor(1, "ph", property, Substitute.For<ITextResolver>(), false);
            sut.AddNumberedParameter(descriptor1);
            Action sutAction = () => sut.AddNumberedParameter(descriptor2);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Descriptor '1' has already been added.", ex.Message);
        }

        [Fact]
        public void AddNamedParameter_DescriptorIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
            Action sutAction = () => sut.AddNamedParameter(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("descriptor", ex.ParamName);
        }

        [Fact]
        public void AddNamedParameter_DescriptorAlreadyAdded_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor1 = new NamedParameterDescriptor("name", "value", property, Substitute.For<ITextResolver>(), false);
            var descriptor2 = new NamedParameterDescriptor("name", "value", property, Substitute.For<ITextResolver>(), false);
            sut.AddNamedParameter(descriptor1);
            Action sutAction = () => sut.AddNamedParameter(descriptor2);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Descriptor 'name' has already been added.", ex.Message);
        }

        [Fact]
        public void AddNamedParameter_DescriptorAlreadyAddedWithDifferentCasing_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor1 = new NamedParameterDescriptor("name", "value", property, Substitute.For<ITextResolver>(), false);
            var descriptor2 = new NamedParameterDescriptor("NAME", "value", property, Substitute.For<ITextResolver>(), false);
            sut.AddNamedParameter(descriptor1);
            Action sutAction = () => sut.AddNamedParameter(descriptor2);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Descriptor 'NAME' has already been added.", ex.Message);
        }

        [Fact]
        public void AddFlagParameter_DescriptorIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
            Action sutAction = () => sut.AddFlagParameter(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("descriptor", ex.ParamName);
        }

        [Fact]
        public void AddFlagParameter_DescriptorAlreadyAdded_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor1 = new FlagParameterDescriptor("name", property, Substitute.For<ITextResolver>(), false);
            var descriptor2 = new FlagParameterDescriptor("name", property, Substitute.For<ITextResolver>(), false);
            sut.AddFlagParameter(descriptor1);
            Action sutAction = () => sut.AddFlagParameter(descriptor2);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Descriptor 'name' has already been added.", ex.Message);
        }

        [Fact]
        public void AddFlagParameter_DescriptorAlreadyAddedWithDifferentCasing_ThrowsException()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor1 = new FlagParameterDescriptor("name", property, Substitute.For<ITextResolver>(), false);
            var descriptor2 = new FlagParameterDescriptor("NAME", property, Substitute.For<ITextResolver>(), false);
            sut.AddFlagParameter(descriptor1);
            Action sutAction = () => sut.AddFlagParameter(descriptor2);

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Descriptor 'NAME' has already been added.", ex.Message);
        }

        [Fact]
        public void Build_WhenCalled_ReturnsCommandDescriptor()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());

            // act
            var commandDescriptor = sut.Build();

            // assert
            Assert.Null(commandDescriptor.CommandIdentifier.Module);
            Assert.Equal("command", commandDescriptor.CommandIdentifier.Name);
            Assert.Equal(commandDescriptor.ImplementingType, GetType());
            Assert.Empty(commandDescriptor.NumberedParameters);
            Assert.Empty(commandDescriptor.NamedParameters);
        }

        [Fact]
        public void Build_CalledWithModule_ReturnsCommandDescriptorIncludingModule()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(new ExecutionTargetIdentifier("module", "command"), GetType(), Substitute.For<ITextResolver>());

            // act
            var commandDescriptor = sut.Build();

            // assert
            Assert.Equal("module", commandDescriptor.CommandIdentifier.Module);
            Assert.Equal("command", commandDescriptor.CommandIdentifier.Name);
            Assert.Equal(commandDescriptor.ImplementingType, GetType());
            Assert.Empty(commandDescriptor.NumberedParameters);
            Assert.Empty(commandDescriptor.NamedParameters);
        }

        [Fact]
        public void Build_AfterNumberedParameterAdded_ReturnsCommandDescriptorWithNumberedParameter()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
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
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor = new NamedParameterDescriptor("name", "value", property, Substitute.For<ITextResolver>(), false);
            sut.AddNamedParameter(descriptor);

            // act
            var commandDescriptor = sut.Build();

            // assert
            Assert.Equal(1, commandDescriptor.NamedParameters.Count);
            Assert.Equal(descriptor, commandDescriptor.NamedParameters["name"]);
        }

        [Fact]
        public void Build_AfterFlagParameterAdded_ReturnsCommandDescriptorWithFlagParameter()
        {
            // arrange
            var sut = new CommandDescriptor.Builder(SampleCommandIdentifier, GetType(), Substitute.For<ITextResolver>());
            var property = CreateProperty();
            var descriptor = new FlagParameterDescriptor("name", property, Substitute.For<ITextResolver>(), false);
            sut.AddFlagParameter(descriptor);

            // act
            var commandDescriptor = sut.Build();

            // assert
            Assert.Equal(1, commandDescriptor.FlagParameters.Count);
            Assert.Equal(descriptor, commandDescriptor.FlagParameters[0]);
        }

        private PropertyInfo CreateProperty()
        {
            return typeof(SampleCommand).GetProperty("Parameter");
        }
    }
}