using System;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
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
        [InlineData("(command)")]
        [InlineData("$command$")]
        [InlineData("[command]")]
        public void Parse_SpecialTokenInCommandName_ThrowsException(string input)
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse(input);

            // act, assert
            Assert.Throws<ParseException>(sutAction);
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
            builder.AddParameter(new Literal("param"));
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
            builder.AddParameter(new Literal("param1"));
            builder.AddParameter(new Literal("param2"));
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
            builder1.AddParameter(new Literal("param1"));
            builder1.AddParameter(new Literal("param2"));
            var expectedCommand1 = builder1.Build();

            var builder2 = new CommandInput.Builder(2, "command");
            builder2.AddParameter(new Literal("param1"));
            builder2.AddParameter(new Literal("param2"));
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
            builder.AddParameter(new ParameterNameCommandParameter("name"));
            builder.AddParameter(new Literal("value"));
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
            builder.AddParameter(new ParameterNameCommandParameter("name"));
            builder.AddParameter(new Literal("value"));
            builder.AddParameter(new ParameterNameCommandParameter("name2"));
            builder.AddParameter(new Literal("value2"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Theory]
        [InlineData("command -")]
        [InlineData("command - num")]
        public void Parse_ParameterNameTokenWithoutName_THrowsException(string input)
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse(input);

            // act, assert
            Assert.Throws<ParseException>(sutAction);
        }

        [Fact]
        public void Parse_BracketedParameterIncludesSpaces_SpacesIncludedInParameter()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter(new Literal("pa ram"));
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
            builder.AddParameter(new Literal("\n\n"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (\n\n)");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_EscapedBrackets_BracketsIncludedInParameter()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter(new Literal("(param)"));
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
            builder.AddParameter(new Literal(@"\"));
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
            builder.AddParameter(new Literal("#"));
            builder.AddParameter(new Literal("param"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(@"command \# param");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_SomeParametersBracketed_ParametersParsedProperly()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter(new Literal(" param  param "));
            builder.AddParameter(new Literal("param"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command ( param  param )  param ");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_NewlineInsideBracketedParameter_ParameterIncludesNewline()
        {
            // arrange
            var sut = new Parser();
            var builder = new CommandInput.Builder(1, "command");
            builder.AddParameter(new Literal("param\nparam"));
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
            builder.AddParameter(new Literal("(param)"));
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
            builder.AddParameter(new Literal("\nic  (pa ram)\nic (pa ram)"));
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
            builder.AddParameter(new Literal("pa ram"));
            builder.AddParameter(new Literal("pa ram"));
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
        public void Parse_MissingClosingBracket_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command (param");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingBlockEnd, exception.Message);
        }

        [Fact]
        public void Parse_MissingOpeningBracket_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command param )");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingBlockStart, exception.Message);
        }

        [Fact]
        public void Parse_UnbalancedBracketOnSecondLine_SourceLineOfExceptionIsCorrect()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command\ncommand (param");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(2, exception.SourceLocation.LineNumber);
        }

        [Fact]
        public void Parse_InputContainsVariable_VariableParsed()
        {
            // arrange
            var builder = new CommandInput.Builder(1, "cmd");
            builder.AddParameter(new VariableReference("var"));
            var expectedCommand = builder.Build();

            var sut = new Parser();

            // act
            var result = sut.Parse("cmd $var$");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_InputContainsUnpairedVariableSymbol_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("cmd $var");

            // act, assert
            var ex = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Unpaired variable token $", ex.Message);
        }

        [Fact]
        public void Parse_InputContainsEscapedVariableSymbol_ParseSymbolAsLiteral()
        {
            // arrange
            var builder = new CommandInput.Builder(1, "cmd");
            builder.AddParameter(new Literal("$var$"));
            var expectedCommand = builder.Build();

            var sut = new Parser();

            // act
            var result = sut.Parse(@"cmd \$var\$");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_InputContainsVariableWithNoName_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("cmd $$");

            // act, assert
            var ex = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Missing variable name", ex.Message);
        }

        [Fact]
        public void Parse_InputContainsParameterName_ParsesParameterName()
        {
            // arrange
            var builder = new CommandInput.Builder(1, "cmd");
            builder.AddParameter(new ParameterNameCommandParameter("par"));
            var expectedCommand = builder.Build();

            var sut = new Parser();

            // act
            var result = sut.Parse("cmd -par");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_InputContainsEscapedParameterNameSymbol_ParsesAsLiteral()
        {
            // arrange
            var builder = new CommandInput.Builder(1, "cmd");
            builder.AddParameter(new Literal("-par"));
            var expectedCommand = builder.Build();

            var sut = new Parser();

            // act
            var result = sut.Parse(@"cmd \-par");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_InputContainsParameterNameSymbol_ParsesAsLiteral()
        {
            // arrange
            var builder = new CommandInput.Builder(1, "cmd");
            builder.AddParameter(new Literal("p-ar"));
            var expectedCommand = builder.Build();

            var sut = new Parser();

            // act
            var result = sut.Parse("cmd p-ar");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_InputContainsParameterNameSymbolInsideBlock_ParsesAsLiteral()
        {
            // arrange
            var builder = new CommandInput.Builder(1, "cmd");
            builder.AddParameter(new Literal("p -ar"));
            var expectedCommand = builder.Build();

            var sut = new Parser();

            // act
            var result = sut.Parse("cmd (p -ar)");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_InputContainsListParameter_ParsesList()
        {
            // arrange
            var expectedList = new List(new ChelType[]{
                new Literal("a"),
                new VariableReference("b")
            });

            var builder = new CommandInput.Builder(1, "cmd");
            builder.AddParameter(expectedList);
            var expectedCommand = builder.Build();

            var sut = new Parser();

            // act
            var result = sut.Parse("cmd [a $b$]");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_InputMissingListEnd_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("cmd [a ");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingListEnd, exception.Message);
        }

        [Fact]
        public void Parse_InputMissingListStart_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("cmd a]");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingListStart, exception.Message);
        }

        [Fact]
        public void Parse_InputContainsEscapedListParameter_ParsesAsLiteral()
        {
            // arrange
            var builder = new CommandInput.Builder(1, "cmd");
            builder.AddParameter(new Literal("[a]"));
            var expectedCommand = builder.Build();

            var sut = new Parser();

            // act
            var result = sut.Parse(@"cmd \[a\]");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        [Fact]
        public void Parse_InputContainsListParameterInbrackets_ParsesAsLiteral()
        {
            // arrange
            var builder = new CommandInput.Builder(1, "cmd");
            builder.AddParameter(new Literal("[a b]"));
            var expectedCommand = builder.Build();

            var sut = new Parser();

            // act
            var result = sut.Parse(@"cmd ([a b])");

            // assert
            Assert.Equal(expectedCommand, result[0], new CommandInputEqualityComparer());
        }

        private CommandInput CreateCommandInput(int sourceLine, string commandName)
        {
            var builder = new CommandInput.Builder(sourceLine, commandName);
            return builder.Build();
        }
    }
}
