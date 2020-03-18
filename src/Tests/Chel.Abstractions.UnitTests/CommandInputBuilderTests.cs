using System;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandInputBuilderTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Ctor_SourceLineIsNonPositive_ThrowsException(int sourceLine)
        {
            // arrange
            Action sutAction = () => new CommandInput.Builder(sourceLine, "command");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("sourceLine", ex.ParamName);
            Assert.Contains("sourceLine must be greater than 0", ex.Message);
        }

        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandInput.Builder(1, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandInput.Builder(1, "");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
            Assert.Contains("commandName cannot be empty", ex.Message);
        }

        [Fact]
        public void AddNumberedParameter_ParameterIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandInput.Builder(1, "command");
            Action sutAction = () => sut.AddNumberedParameter(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void AddNamedParameter_ParameterNameIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandInput.Builder(1, "command");
            Action sutAction = () => sut.AddNamedParameter(null, "value");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void AddNamedParameter_ParameterNameIsEmptyOrWhiteSpace_ThrowsException(string name)
        {
            // arrange
            var sut = new CommandInput.Builder(1, "command");
            Action sutAction = () => sut.AddNamedParameter(name, "value");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
            Assert.Contains("name cannot be empty or whitespace", ex.Message);
        }

        [Fact]
        public void AddNamedParameter_ParameterAlreadyAdded_ThrowsException()
        {
            // arrange
            var sut = new CommandInput.Builder(1, "command");
            sut.AddNamedParameter("pname", "value1");
            Action sutAction = () => sut.AddNamedParameter("pname", "value2");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
            Assert.Contains("pname has already been added", ex.Message);
        }

        [Fact]
        public void AddNamedParameter_ParameterAlreadyAddedWithDifferentCase_ThrowsException()
        {
            // arrange
            var sut = new CommandInput.Builder(1, "command");
            sut.AddNamedParameter("pname", "value1");
            Action sutAction = () => sut.AddNamedParameter("PNAME", "value2");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
            Assert.Contains("PNAME has already been added", ex.Message);
        }

        [Fact]
        public void AddNamedParameter_ParameterValueIsNull_ThrowsException()
        {
            // arrange
            var sut = new CommandInput.Builder(1, "command");
            Action sutAction = () => sut.AddNamedParameter("name", null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void Build_WhenCalled_ReturnsCommandInput()
        {
            // arrange
            var sut = new CommandInput.Builder(3, "command");

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal(3, commandInput.SourceLine);
            Assert.Equal("command", commandInput.CommandName);
        }

        [Fact]
        public void Build_AfterAddNumberedParameterCalled_ReturnsCommandInputWithNumberedParameter()
        {
            // arrange
            var sut = new CommandInput.Builder(3, "command");
            sut.AddNumberedParameter("value1");
            sut.AddNumberedParameter("value2");

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal("value1", commandInput.NumberedParameters[0]);
            Assert.Equal("value2", commandInput.NumberedParameters[1]);
        }

        [Fact]
        public void Build_AfterAddNamedParameterCalled_ReturnsCommandInputWithNamedParameter()
        {
            // arrange
            var sut = new CommandInput.Builder(3, "command");
            sut.AddNamedParameter("name1", "value1");
            sut.AddNamedParameter("name2", "value2");

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal("value1", commandInput.NamedParameters["name1"]);
            Assert.Equal("value2", commandInput.NamedParameters["name2"]);
        }

        [Fact]
        public void Build_AfterAddNamedParameterCalled_CanAccessParameterWithDifferentCasing()
        {
            // arrange
            var sut = new CommandInput.Builder(3, "command");
            sut.AddNamedParameter("name1", "value1");
            sut.AddNamedParameter("name2", "value2");

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal("value1", commandInput.NamedParameters["Name1"]);
            Assert.Equal("value2", commandInput.NamedParameters["NAME2"]);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData(" ")]
        public void Build_EmptyOrWhiteSpaceNamedParameters_ReturnsCommandInputWithNamedParameter(string value)
        {
            // arrange
            var sut = new CommandInput.Builder(3, "command");
            sut.AddNamedParameter("name", value);

            // act
            var commandInput = sut.Build();

            // assert
            Assert.Equal(value, commandInput.NamedParameters["name"]);
        }
    }
}