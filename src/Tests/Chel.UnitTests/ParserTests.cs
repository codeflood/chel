using System;
using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions;
using Chel.Exceptions;
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
            var expected = new[] { CreateCommandInput(1, "command") };

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
            var expected = new[] { CreateCommandInput(sourceLine1, "command"), CreateCommandInput(sourceLine2, "command") };

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
            var expected = new[] { CreateCommandInput(sourceLine, "command") };

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("command param")]
        [InlineData("command   param  ")]
        [InlineData("command\tparam")]
        [InlineData("command \t param \t")]
        public void Parse_SingleNumberedParameter_ParameterParsed(string input)
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("param");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Theory]
        [InlineData("command param1 param2")]
        [InlineData("command\tparam1\tparam2  ")]
        [InlineData("command  \t  param1  \t  param2  \t")]
        public void Parse_TwoNumberedParameters_ParametersParsed(string input)
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("param1");
            builder.AddNumberedParameter("param2");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Theory]
        [InlineData("command param1 param2\ncommand param1 param2")]
        [InlineData("command\tparam1\tparam2  \ncommand\tparam1\tparam2  ")]
        [InlineData("command  \t  param1  \t  param2  \t\ncommand  \t  param1  \t  param2  \t")]
        public void Parse_TwoCommandsTwoNumberedParameters_ParsedCommandsWithParameters(string input)
        {
            // arrange
            var sut = new Parser();
            
            var builder1 = new CommandInput.Builder(1, "command");
            builder1.AddNumberedParameter("param1");
            builder1.AddNumberedParameter("param2");
            var expectedCommand1 = builder1.Build();

            var builder2 = new CommandInput.Builder(2, "command");
            builder2.AddNumberedParameter("param1");
            builder2.AddNumberedParameter("param2");
            var expectedCommand2 = builder2.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(new[] { expectedCommand1, expectedCommand2 }, result);
        }

        [Fact]
        public void Parse_ParenthesisedParameterIncludesSpaces_SpacesIncludedInParameter()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("pa ram");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (pa ram)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_ParameterIsOnlyNewlines_NewlinesIncludedInParameter()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("\n\n");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (\n\n)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_EscapedParentheses_ParenthesesIncludedInParameter()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("(param)");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(@"command \(param\)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_EscapedCommentSymbol_CommentSymbolIncludedInParameter()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("#");
            builder.AddNumberedParameter("param");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(@"command \# param");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_SomeParametersParenthesised_ParametersParsedProperly()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter(" param  param ");
            builder.AddNumberedParameter("param");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command ( param  param )  param ");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_NewlineInsideParenthesisedParameter_ParameterIncludesNewline()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("param\nparam");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (param\nparam)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_DoubleParenthesesInParameters_ParameterIncludesParentheses()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("(param)");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command ((param))");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_NestedParenthesesInParameters_ParameterIncludesParentheses()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("\nic  (pa ram)\nic (pa ram)");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (\nic  (pa ram)\nic (pa ram))");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_MultipleBracketedParameters_ParametersParsedProperly()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddNumberedParameter("pa ram");
            builder.AddNumberedParameter("pa ram");
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (pa ram) (pa ram)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_ErrorOnLineTwo_ReportsCorrectLineNumberInError()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command (\npa ram");

            // act, assert
            var exception = Assert.Throws<ParserException>(sutAction);
            Assert.Equal(2, exception.SourceLine);
        }

        [Fact]
        public void Parse_ErrorOnLineThree_ReportsCorrectLineNumberInError()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command (\npa ram\nram");

            // act, assert
            var exception = Assert.Throws<ParserException>(sutAction);
            Assert.Equal(3, exception.SourceLine);
        }

        [Fact]
        public void Parse_MissingClosingParenthesis_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command (param");

            // act, assert
            var exception = Assert.Throws<ParserException>(sutAction);
            Assert.Equal(Texts.MissingClosingParenthesis, exception.Message);
        }

        [Fact]
        public void Parse_MissingOpeningParenthesis_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command param )");

            // act, assert
            var exception = Assert.Throws<ParserException>(sutAction);
            Assert.Equal(Texts.MissingOpeningParenthesis, exception.Message);
        }

        [Fact]
        public void Parse_UnbalancedParenthesisOnSecondLine_SourceLineOfExceptionIsCorrect()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command\ncommand (param");

            // act, assert
            var exception = Assert.Throws<ParserException>(sutAction);
            Assert.Equal(2, exception.SourceLine);
        }

        private CommandInput CreateCommandInput(int sourceLine, string commandName)
        {
            var builder = new CommandInput.Builder(sourceLine, commandName);
            return builder.Build();
        }
    }
}
