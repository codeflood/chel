using System;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandDescriptorBuilderTests
    {
        [Fact]
        public void Ctor_ImplementingTypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(null, "command", "lorem ipsum");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("implementingType", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(GetType(), null, "lorem ipsum");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(GetType(), "", "lorem ipsum");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
            Assert.Contains("commandName cannot be empty", ex.Message);
        }

        [Fact]
        public void Ctor_DescriptionIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(GetType(), "command", null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("description", ex.ParamName);
        }

        [Fact]
        public void Ctor_DescriptionIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor.Builder(GetType(), "command", "");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("description", ex.ParamName);
            Assert.Contains("description cannot be empty", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = CreateCommandDescriptorBuilder();

            // assert
            Assert.Equal(GetType(), sut.ImplementingType);
            Assert.Equal("command", sut.CommandName);
            Assert.Equal("lorem ipsum", sut.Description);
        }

        [Fact]
        public void Build_TypeIsNull_ThrowsException()
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();
            sut.ImplementingType = null;

            Action sutAction = () => sut.Build();

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("ImplementingType cannot be null", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Build_CommandNameIsNullOrEmpty_ThrowsException(string commandName)
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();
            sut.CommandName = commandName;

            Action sutAction = () => sut.Build();

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("CommandName cannot be null or empty", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Build_DescriptionIsNullOrEmpty_ThrowsException(string description)
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();
            sut.Description = description;

            Action sutAction = () => sut.Build();

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Description cannot be null or empty", ex.Message);
        }

        [Fact]
        public void Build_WhenCalled_ReturnsCommandDescriptor()
        {
            // arrange
            var sut = CreateCommandDescriptorBuilder();
            sut.Description = "lorem ipsum";

            // act
            var descriptor = sut.Build();

            // assert
            Assert.Equal(GetType(), descriptor.ImplementingType);
            Assert.Equal("command", descriptor.CommandName);
            Assert.Equal("lorem ipsum", descriptor.Description);
        }

        private CommandDescriptor.Builder CreateCommandDescriptorBuilder()
        {
            return new CommandDescriptor.Builder(GetType(), "command", "lorem ipsum");
        }
    }
}