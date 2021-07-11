using System;
using Chel.Abstractions;
using Chel.Exceptions;
using Chel.Parsing;
using Chel.UnitTests.Comparers;
using Xunit;

namespace Chel.UnitTests.Parsing
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
            var expected = new[] { CreateCommandInput(1, "command") };

            // act
            var result = sut.Parse("command");

            // assert
            Assert.Equal(expected, result, new CommandInputEqualityComparer());
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
            var expected = new[] { CreateCommandInput(sourceLine1, "command"), CreateCommandInput(sourceLine2, "command") };

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expected, result, new CommandInputEqualityComparer());
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
            var expected = new[] { CreateCommandInput(sourceLine, "command") };

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expected, result, new CommandInputEqualityComparer());
        }

        [Theory]
        [InlineData("command param")]
        [InlineData("command   param  ")]
        [InlineData("command\tparam")]
        [InlineData("command \t param \t")]
        public void Parse_SingleParameter_ParameterParsed(string input)
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("param");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Theory]
        [InlineData("command param1 param2")]
        [InlineData("command\tparam1\tparam2  ")]
        [InlineData("command  \t  param1  \t  param2  \t")]
        public void Parse_TwoParameters_ParametersParsed(string input)
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("param1");
            builder.AddParameter("param2");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Theory]
        [InlineData("command param1 param2\ncommand param1 param2")]
        [InlineData("command\tparam1\tparam2  \ncommand\tparam1\tparam2  ")]
        [InlineData("command  \t  param1  \t  param2  \t\ncommand  \t  param1  \t  param2  \t")]
        public void Parse_TwoCommandsTwoParameters_ParsedCommandsWithParameters(string input)
        {
            // arrange
            var sut = new Parser();
            
            var builder1 = new CommandInput.Builder(1, "command");
            builder1.AddParameter("param1");
            builder1.AddParameter("param2");
            var expectedCommand1 = builder1.Build();

            var builder2 = new CommandInput.Builder(2, "command");
            builder2.AddParameter("param1");
            builder2.AddParameter("param2");
            var expectedCommand2 = builder2.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(new[] { expectedCommand1, expectedCommand2 }, result, new CommandInputEqualityComparer());
        }

        [Theory]
        [InlineData("command -name value")]
        [InlineData("command  -name\tvalue")]
        public void Parse_NamedParameters_ParameterParsed(string input)
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("-name");
            builder.AddParameter("value");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Theory]
        [InlineData("command -name value -name2 value2")]
        [InlineData("command  -name\tvalue   -name2\tvalue2")]
        public void Parse_MultipleNamedParameters_ParameterParsed(string input)
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("-name");
            builder.AddParameter("value");
            builder.AddParameter("-name2");
            builder.AddParameter("value2");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_ParenthesisedParameterIncludesSpaces_SpacesIncludedInParameter()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("pa ram");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (pa ram)");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_ParameterIsOnlyNewlines_NewlinesIncludedInParameter()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("\n\n");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (\n\n)");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_EscapedParentheses_ParenthesesIncludedInParameter()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("(param)");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(@"command \(param\)");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_ParameterIncludesSlash_ParameterParsed()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter(@"\");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(@"command \\");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_EscapedCommentSymbol_CommentSymbolIncludedInParameter()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("#");
            builder.AddParameter("param");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(@"command \# param");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_SomeParametersParenthesised_ParametersParsedProperly()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter(" param  param ");
            builder.AddParameter("param");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command ( param  param )  param ");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_NewlineInsideParenthesisedParameter_ParameterIncludesNewline()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("param\nparam");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (param\nparam)");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_EscapedBracketsInParameters_ParameterIncludesBrackets()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("(param)");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (\\(param\\))");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_NestedBracketsInParameters_ParameterIncludesBrackets()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("\nic  (pa ram)\nic (pa ram)");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (\nic  \\(pa ram\\)\nic \\(pa ram\\))");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_MultipleBracketedParameters_ParametersParsedProperly()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter("pa ram");
            builder.AddParameter("pa ram");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (pa ram) (pa ram)");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_ErrorOnLineTwo_ReportsCorrectLineNumberInError()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command (\npa ram");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(2, exception.SourceLocation.LineNumber);
        }

        [Fact]
        public void Parse_ErrorOnLineThree_ReportsCorrectLineNumberInError()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command (\npa ram\nram");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(3, exception.SourceLocation.LineNumber);
        }

        [Fact]
        public void Parse_MissingClosingParenthesis_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command (param");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingBlockEnd, exception.Message);
        }

        [Fact]
        public void Parse_MissingOpeningParenthesis_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command param )");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingBlockStart, exception.Message);
        }

        [Fact]
        public void Parse_UnbalancedParenthesisOnSecondLine_SourceLineOfExceptionIsCorrect()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command\ncommand (param");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(2, exception.SourceLocation.LineNumber);
        }

        private CommandInput CreateCommandInput(int sourceLine, string commandName)
        {
            var builder = new CommandInput.Builder(sourceLine, commandName);
            return builder.Build();
        }
    }
}
