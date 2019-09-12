using System;
using Chel;
using Chel.Abstractions;
using Xunit;

namespace Chel.UnitTests
{
    public class ParserTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\r\n")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("   ")]
        public void Parse_EmptyInput_ReturnsEmptyCommandInputList(string input)
        {
            // arrange
            var sut = new Parser();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_SingleCommand_ReturnsCommandInput()
        {
            // arrange
            var sut = new Parser();
            var expected = new[] { new CommandInput("command") };

            // act
            var result = sut.Parse("command");

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("command\ncommand")]
        [InlineData("command\r\ncommand")]
        [InlineData(" command  \t   \r\n   command  \r\n\r\n")]
        public void Parse_MultipleCommands_ReturnsCommandInputs(string input)
        {
            // arrange
            var sut = new Parser();
            var expected = new[] { new CommandInput("command"), new CommandInput("command") };

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expected, result);
        }
    }
}
