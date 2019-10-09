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
            var expected = new[] { new CommandInput(1, "command") };

            // act
            var result = sut.Parse("command");

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("command\ncommand", 1, 2)]
        [InlineData("\n\ncommand\n\n\n\n\ncommand\n\n", 3, 8)]
        [InlineData("command\r\ncommand", 1, 2)]
        [InlineData(" command  \t   \r\n   command  \r\n\r\n", 1, 2)]
        public void Parse_MultipleCommands_ReturnsCommandInputs(string input, int sourceLine1, int sourceLine2)
        {
            // arrange
            var sut = new Parser();
            var expected = new[] { new CommandInput(sourceLine1, "command"), new CommandInput(sourceLine2, "command") };

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("command#comment", 1)]
        [InlineData("command  # comment", 1)]
        [InlineData("# comment\r\ncommand", 2)]
        [InlineData("# comment\r\ncommand\r\n# comment", 2)]
        public void Parse_InputContainsComments_CommentsAreIgnored(string input, int sourceLine)
        {
            // arrange
            var sut = new Parser();
            var expected = new[] { new CommandInput(sourceLine, "command") };

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expected, result);
        }
    }
}
