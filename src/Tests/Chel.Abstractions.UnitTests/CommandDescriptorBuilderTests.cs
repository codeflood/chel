using System;
using System.Globalization;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandDescriptorBuilderTests
    {
        [Fact]
        public void Ctor_ImplementingTypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(null, "command");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("implementingType", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(GetType(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(GetType(), "");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
            Assert.Contains("commandName cannot be empty", ex.Message);
        }

        [Fact]
        public void AddDescription_DescriptionIsNull_ThrowsException()
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();
            Action sutAction = () => sut.AddDescription(null, "en-AU");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("description", ex.ParamName);
        }

        [Fact]
        public void AddDescription_DescriptionIsEmpty_ThrowsException()
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();
            Action sutAction = () => sut.AddDescription("", "en-AU");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("description", ex.ParamName);
            Assert.Contains("description cannot be empty", ex.Message);
        }

        [Fact]
        public void AddDescription_CultureNameIsEmpty_DescriptionSetForInvariantCulture()
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();

            // act
            sut.AddDescription("description", "");

            // assert
            var descriptor = sut.Build();
            var description = descriptor.GetDescription(CultureInfo.InvariantCulture.Name);
            Assert.Equal("description", description);
        }

        [Fact]
        public void AddDescription_CultureAlreadyAdded_ThrowsException()
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();
            sut.AddDescription("description", "en");
            Action sutAction = () => sut.AddDescription("description", "en");

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Description for culture en has already been added", ex.Message);
        }

        [Fact]
        public void Build_LocalizedDescriptionsAdded_DescriptionsAvailableOnDescriptor()
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();
            sut.AddDescription("description", "en");
            sut.AddDescription("au description", "en-AU");

            // act
            var descriptor = sut.Build();

            // assert
            Assert.Equal("description", descriptor.GetDescription("en"));
            Assert.Equal("au description", descriptor.GetDescription("en-AU"));
        }

        [Fact]
        public void Build_WhenCalled_ReturnsCommandDescriptor()
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();
            sut.AddDescription("description", "en");

            // act
            var descriptor = sut.Build();

            // assert
            Assert.Equal(GetType(), descriptor.ImplementingType);
            Assert.Equal("command", descriptor.CommandName);
            Assert.Equal("description", descriptor.GetDescription("en"));
        }

        [Fact]
        public void Build_NoDescriptionsAdded_ReturnsDescriptor()
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();

            // act
            var descriptor = sut.Build();

            // act, assert
            Assert.Equal(GetType(), descriptor.ImplementingType);
            Assert.Equal("command", descriptor.CommandName);
        }

        private CommandDescriptor.Builder CreateCommandDescriptorBuilder()
        {
            return new CommandDescriptor.Builder(GetType(), "command");
        }
    }
}