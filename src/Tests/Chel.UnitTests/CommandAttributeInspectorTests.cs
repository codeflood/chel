using System;
using System.Globalization;
using Chel.Exceptions;
using Chel.UnitTests.SampleCommands;
using Xunit;

namespace Chel.UnitTests
{
    public class CommandAttributeInspectorTests
    {
        [Fact]
        public void DescribeCommand_CommandTypeIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandAttributeInspector();
            Action sutAction = () => sut.DescribeCommand(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandType", ex.ParamName);
        }

        [Fact]
        public void DescribeCommand_TypeIsNotACommand_ThrowsException()
        {
            // arrange
            var sut = new CommandAttributeInspector();
            var type = GetType();
            Action sutAction = () => sut.DescribeCommand(type);

            // act, assert
            var ex = Assert.Throws<TypeNotACommandException>(sutAction);
            Assert.Equal(type, ex.Type);
        }

        [Fact]
        public void DescribeCommand_CommandWithoutDescription_ReturnsDescriptor()
        {
            // arrange
            var sut = new CommandAttributeInspector();

            // act
            var descriptor = sut.DescribeCommand(typeof(SampleCommand2));

            // assert
            Assert.Equal(typeof(SampleCommand2), descriptor.ImplementingType);
            Assert.Equal("sample2", descriptor.CommandName);

            var description = descriptor.GetDescription(CultureInfo.InvariantCulture.Name);
            Assert.Null(description);
        }

        [Fact]
        public void DescribeCommand_CommandWithDescriptions_ReturnsDescriptor()
        {
            // arrange
            var sut = new CommandAttributeInspector();

            // act
            var descriptor = sut.DescribeCommand(typeof(SampleCommand));

            // assert
            Assert.Equal(typeof(SampleCommand), descriptor.ImplementingType);
            Assert.Equal("sample", descriptor.CommandName);

            var invariantDescription = descriptor.GetDescription(CultureInfo.InvariantCulture.Name);
            Assert.Equal("description", invariantDescription);

            var enDescription = descriptor.GetDescription("en");
            Assert.Equal("description", enDescription);

            var deDescription = descriptor.GetDescription("de");
            Assert.Equal("das description", deDescription);
        }

        [Fact]
        public void DescribeCommand_CommandHasNumberedParameters_ReturnsDescriptor()
        {
            // arrange
            var sut = new CommandAttributeInspector();

            // act
            var descriptor = sut.DescribeCommand(typeof(NumberedParameterCommand));

            // assert
            Assert.Equal(typeof(NumberedParameterCommand), descriptor.ImplementingType);
            Assert.Equal("num", descriptor.CommandName);

            var numberedParameterDescriptor1 = descriptor.NumberedParameters[0];
            Assert.Equal(1, numberedParameterDescriptor1.Number);
            Assert.Equal("param1", numberedParameterDescriptor1.PlaceholderText);
            Assert.Equal(typeof(NumberedParameterCommand).GetProperty("NumberedParameter1"), numberedParameterDescriptor1.Property);
            Assert.Equal("The first parameter", numberedParameterDescriptor1.GetDescription(""));

            var numberedParameterDescriptor2 = descriptor.NumberedParameters[1];
            Assert.Equal(2, numberedParameterDescriptor2.Number);
            Assert.Equal("param2", numberedParameterDescriptor2.PlaceholderText);
            Assert.Equal(typeof(NumberedParameterCommand).GetProperty("NumberedParameter2"), numberedParameterDescriptor2.Property);
            Assert.Equal("The second parameter", numberedParameterDescriptor2.GetDescription(""));
        }
    }
}