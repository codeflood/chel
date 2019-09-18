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
        [InlineData("\n\ncommand\n\n\n\n\ncommand\n\n")]
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

        [Theory]
        [InlineData("command#comment")]
        [InlineData("command  # comment")]
        [InlineData("# comment\r\ncommand")]
        [InlineData("# comment\r\ncommand\r\n# comment")]
        public void Parse_InputContainsComments_CommentsAreIgnored(string input)
        {
            // arrange
            var sut = new Parser();
            var expected = new[] { new CommandInput("command") };

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expected, result);
        }
    }
}
