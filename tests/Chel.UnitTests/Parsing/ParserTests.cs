using System;
using System.Collections.Generic;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
using Chel.Exceptions;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
	public class ParserTests
    {
        private INameValidator _nameValidator = new NameValidator();

        [Fact]
        public void Ctor_NameValidatorIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Parser(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("nameValidator", ex.ParamName);
        }

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
            var sut = CreateParser();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_SingleCommand_ReturnsCommandInput()
        {
            // arrange
            var sut = CreateParser();
            var expected = new[] { CreateCommandInput(1, 1, "command") };

            // act
            var result = sut.Parse("command");

            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Parse_SingleCommandNameWithDash_ReturnsCommandInput()
        {
            // arrange
            var sut = CreateParser();
            var expected = new[] { CreateCommandInput(1, 1, "comm-and") };

            // act
            var result = sut.Parse("comm-and");

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("(command)")]
        [InlineData("$command$")]
        [InlineData("[command]")]
        public void Parse_SpecialTokenInCommandName_ThrowsException(string input)
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse(input);

            // act, assert
            Assert.Throws<ParseException>(sutAction);
        }

        [Theory]
        [InlineData("command\ncommand", 1, 1, 2, 1)]
        [InlineData("\n\ncommand\n\n\n\n\ncommand\n\n", 3, 1, 8, 1)]
        [InlineData("command\r\ncommand", 1, 1, 2, 1)]
        [InlineData(" command  \t   \r\n   command  \r\n\r\n", 1, 2, 2, 4)]
        public void Parse_MultipleCommands_ReturnsCommandInputs(string input, int sourceLine1, int sourceCharacter1, int sourceLine2, int sourceCharacter2)
        {
            // arrange
            var sut = CreateParser();
            var expected = new[] { CreateCommandInput(sourceLine1, sourceCharacter1, "command"), CreateCommandInput(sourceLine2, sourceCharacter2, "command") };

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("command#comment", 1, 1)]
        [InlineData("command  # comment", 1, 1)]
        [InlineData("# comment\r\ncommand", 2, 1)]
        [InlineData("# comment\r\ncommand\r\n# comment", 2, 1)]
        public void Parse_InputContainsComments_CommentsAreIgnored(string input, int sourceLine, int sourceCharacter)
        {
            // arrange
            var sut = CreateParser();
            var expected = new[] { CreateCommandInput(sourceLine, sourceCharacter, "command") };

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
        public void Parse_SingleParameter_ParameterParsed(string input)
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal("param"));
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
        public void Parse_TwoParameters_ParametersParsed(string input)
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal("param1"));
            builder.AddParameter(new Literal("param2"));
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
        public void Parse_TwoCommandsTwoParameters_ParsedCommandsWithParameters(string input)
        {
            // arrange
            var sut = CreateParser();
            
            var location1 = new SourceLocation(1, 1);
            var builder1 = new CommandInput.Builder(location1, "command");
            builder1.AddParameter(new Literal("param1"));
            builder1.AddParameter(new Literal("param2"));
            var expectedCommand1 = builder1.Build();

            var location2 = new SourceLocation(2, 1);
            var builder2 = new CommandInput.Builder(location2, "command");
            builder2.AddParameter(new Literal("param1"));
            builder2.AddParameter(new Literal("param2"));
            var expectedCommand2 = builder2.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(new[] { expectedCommand1, expectedCommand2 }, result);
        }

        [Theory]
        [InlineData("command -name value")]
        [InlineData("command  -name\tvalue")]
        public void Parse_NamedParameters_ParameterParsed(string input)
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new ParameterNameCommandParameter("name"));
            builder.AddParameter(new Literal("value"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Theory]
        [InlineData("command -name value -name2 value2")]
        [InlineData("command  -name\tvalue   -name2\tvalue2")]
        public void Parse_MultipleNamedParameters_ParameterParsed(string input)
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new ParameterNameCommandParameter("name"));
            builder.AddParameter(new Literal("value"));
            builder.AddParameter(new ParameterNameCommandParameter("name2"));
            builder.AddParameter(new Literal("value2"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Theory]
        [InlineData("command -")]
        [InlineData("command - num")]
        public void Parse_ParameterNameTokenWithoutName_ThrowsException(string input)
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse(input);

            // act, assert
            Assert.Throws<ParseException>(sutAction);
        }

        [Fact]
        public void Parse_BracketedParameterIncludesSpaces_SpacesIncludedInParameter()
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal("pa ram"));
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
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal("\n\n"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (\n\n)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_EscapedBrackets_BracketsIncludedInParameter()
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal("(param)"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(@"command \(param\)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_ParameterIncludesSlash_ParameterParsed()
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal(@"\"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(@"command \\");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_EscapedCommentSymbol_CommentSymbolIncludedInParameter()
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal("#"));
            builder.AddParameter(new Literal("param"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(@"command \# param");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_SomeParametersBracketed_ParametersParsedProperly()
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal(" param  param "));
            builder.AddParameter(new Literal("param"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command ( param  param )  param ");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_NewlineInsideBracketedParameter_ParameterIncludesNewline()
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal("param\nparam"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (param\nparam)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_EscapedBracketsInParameters_ParameterIncludesBrackets()
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal("(param)"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (\\(param\\))");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_NestedBracketsInParameters_ParameterIncludesBrackets()
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal("\nic  (pa ram)\nic (pa ram)"));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse("command (\nic  \\(pa ram\\)\nic \\(pa ram\\))");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_MultipleBracketedParameters_ParametersParsedProperly()
        {
            // arrange
            var sut = CreateParser();
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "command");
            builder.AddParameter(new Literal("pa ram"));
            builder.AddParameter(new Literal("pa ram"));
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
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("command (\npa ram");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(2, exception.SourceLocation.LineNumber);
        }

        [Fact]
        public void Parse_ErrorOnLineThree_ReportsCorrectLineNumberInError()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("command (\npa ram\nram");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(3, exception.SourceLocation.LineNumber);
        }

        [Fact]
        public void Parse_MissingClosingBracket_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("command (param");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingBlockEnd, exception.Message);
        }

        [Fact]
        public void Parse_MissingOpeningBracket_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("command param )");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingBlockStart, exception.Message);
        }

        [Fact]
        public void Parse_UnbalancedBracketOnSecondLine_SourceLineOfExceptionIsCorrect()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("command\ncommand (param");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(2, exception.SourceLocation.LineNumber);
        }

        [Fact]
        public void Parse_InputContainsVariable_VariableParsed()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new VariableReference("var"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd $var$");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsUnpairedVariableSymbol_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd $var");

            // act, assert
            var ex = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Unpaired variable token $", ex.Message);
        }

        [Fact]
        public void Parse_InputContainsEscapedVariableSymbol_ParseSymbolAsLiteral()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new Literal("$var$"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(@"cmd \$var\$");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsVariableWithNoName_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd $$");

            // act, assert
            var ex = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Missing variable name", ex.Message);
        }

        [Fact]
        public void Parse_InputContainsVariableWithSubReference_ParsesVariableAndSubReference()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new VariableReference("var", new[] { "1" }));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd $var:1$");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsVariableWithSubSubReference_ParsesVariableAndSubSubReference()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new VariableReference("var", new[] { "1", "2" }));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd $var:1:2$");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsVariableWithMissingSubReference_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd $var:$");

            // act, assert
            var ex = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Missing subreference for variable $var$", ex.Message);
        }

        [Fact]
        public void Parse_InputContainsParameterName_ParsesParameterName()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new ParameterNameCommandParameter("par"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd -par");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsEscapedParameterNameSymbol_ParsesAsLiteral()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new Literal("-par"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(@"cmd \-par");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsParameterNameSymbol_ParsesAsLiteral()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new Literal("p-ar"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd p-ar");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsParameterNameSymbolInsideBlock_ParsesAsLiteral()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new Literal("p -ar"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd (p -ar)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsListParameter_ParsesList()
        {
            // arrange
            var expectedList = new List(new ChelType[]{
                new Literal("a"),
                new VariableReference("b")
            });

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(expectedList);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd [a $b$]");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputMissingListEnd_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd [a ");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingListEnd, exception.Message);
        }

        [Fact]
        public void Parse_InputMissingListStart_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd a]");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingListStart, exception.Message);
        }

        [Fact]
        public void Parse_InputContainsEscapedListParameter_ParsesAsLiteral()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new Literal("[a]"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(@"cmd \[a\]");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsListParameterInBrackets_ParsesAsLiteral()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new Literal("[a b]"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(@"cmd ([a b])");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsListWithMap_ParsesAsLiteral()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");

            var map1Entries = new Dictionary<string, ICommandParameter> { { "a", new Literal("b") }};
            var map2Entries = new Dictionary<string, ICommandParameter> { { "c", new Literal("d") }};

            builder.AddParameter(new List(new[]{ new Map(map1Entries), new Map(map2Entries) }));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(@"cmd [{a:b}   { c : d } ]");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Theory]
        [MemberData(nameof(Parse_InputContainsMapParameter_ParsesMap_DataSource))]
        public void Parse_InputContainsMapParameter_ParsesMap(string input)
        {
            // arrange
            var expectedMap = new Map(new Dictionary<string, ICommandParameter>
            {
                { "a", new Literal("b")}
            });

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(expectedMap);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        public static IEnumerable<object[]> Parse_InputContainsMapParameter_ParsesMap_DataSource()
        {
            yield return new[] { "cmd {a: b}" };
            yield return new[] { "cmd { a: b }" };
            yield return new[] { "cmd {a:b}" };
            yield return new[] { "cmd {     a        :      b        }" };
            yield return new[] { "cmd {\n  a:  b  \n}" };
        }

        [Theory]
        [MemberData(nameof(Parse_InputContainsMapParameterWithVariableReference_ParsesMap_DataSource))]
        public void Parse_InputContainsMapParameterWithVariableReference_ParsesMap(string input)
        {
            // arrange
            var expectedMap = new Map(new Dictionary<string, ICommandParameter>
            {
                { "a", new VariableReference("b")}
            });

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(expectedMap);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        public static IEnumerable<object[]> Parse_InputContainsMapParameterWithVariableReference_ParsesMap_DataSource()
        {
            yield return new[] { "cmd {a: $b$}" };
            yield return new[] { "cmd {a:$b$}" };
            yield return new[] { "cmd {     a        :      $b$        }" };
            yield return new[] { "cmd {\n  a:  $b$  \n}" };
        }

        [Fact]
        public void Parse_InputContainsMapParameterWithManyEntries_ParsesMap()
        {
            // arrange
            var expectedMap = new Map(new Dictionary<string, ICommandParameter>
            {
                { "foo", new Literal("bar")},
                { "num", new Literal("12")}
            });

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(expectedMap);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd {foo: bar num : 12}");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsMapParameterWithManyEntriesSpacesInValues_ParsesMap()
        {
            // arrange
            var expectedMap = new Map(new Dictionary<string, ICommandParameter>
            {
                { "foo", new Literal("bar baz")},
                { "num", new Literal("12")}
            });

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(expectedMap);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd {foo: (bar baz) num : 12}");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsMapParameterWithManyEntriesMultiLine_ParsesMap()
        {
            // arrange
            var expectedMap = new Map(new Dictionary<string, ICommandParameter>
            {
                { "foo", new Literal("bar baz")},
                { "num", new Literal("12")}
            });

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(expectedMap);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(@"cmd {
                foo: (bar baz)
                num : 12
}");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsMapWithDashInName_ParsesMap()
        {
            // arrange
            var expectedMap = new Map(new Dictionary<string, ICommandParameter>
            {
                { "the-key", new Literal("the-value")}
            });

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(expectedMap);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd { the-key: the-value }");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputMissingMapEnd_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd {a: b");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingMapEnd, exception.Message);
        }

        [Fact]
        public void Parse_InputMissingMapStart_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd a:b}");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingMapStart, exception.Message);
        }

        [Fact]
        public void Parse_InputContainsEscapedMapParameter_ParsesAsLiteral()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new Literal("{a:b}"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(@"cmd \{a:b\}");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsMapParameterInBrackets_ParsesAsLiteral()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new Literal("{a: b}"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(@"cmd ({a: b})");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsMapWithList_ParsesAsLiteral()
        {
            // arrange
            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");

            var expectedMap1 = new Map(new Dictionary<string, ICommandParameter>
            {
                { "a", new List(new[]{ new Literal("1"), new Literal("2") })},
                { "b", new List(new[]{ new Literal("3"), new Literal("4") })}
            });

            builder.AddParameter(expectedMap1);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(@"cmd {a: [1 2]  b : [ 3 4 ]}");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Theory]
        [MemberData(nameof(Parse_MapWithInvalidEntryName_ThrowsException_DataSource))]
        public void Parse_MapWithInvalidEntryName_ThrowsException(string input)
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse(input);

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Invalid character in map entry name", exception.Message);
        }

        public static IEnumerable<object[]> Parse_MapWithInvalidEntryName_ThrowsException_DataSource()
        {
            yield return new[]{ "cmd { a\\\\a: b }" };
            yield return new[]{ "cmd { a\\(a: b }" };
        }

        [Theory]
        [MemberData(nameof(Parse_MapWithSpecialSymbolsInEntryName_ThrowsException_DataSource))]
        public void Parse_MapWithSpecialSymbolsInEntryName_ThrowsException(string input)
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse(input);

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Missing map entry name", exception.Message);
        }

        public static IEnumerable<object[]> Parse_MapWithSpecialSymbolsInEntryName_ThrowsException_DataSource()
        {
            yield return new[]{ "cmd { -a: b }" };
            yield return new[]{ "cmd { (a b): b }" };
            yield return new[]{ "cmd { $a: b }" };
            yield return new[]{ "cmd { a$: b }" };
        }

        [Fact]
        public void Parse_MapMissingName_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd { : b }");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingMapEntryName, exception.Message);
        }

        [Fact]
        public void Parse_MapMissingNameDelimiter_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd { a b }");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingMapEntryName, exception.Message);
        }

        [Fact]
        public void Parse_MapMissingValue_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd { a: }");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(Texts.MissingMapEntryValue, exception.Message);
        }

        [Fact]
        public void Parse_InputContainsSeparatedSubcommand_ParsesSubcommand()
        {
            // arrange
            var sublocation = new SourceLocation(1, 8);
            var subcommand = new CommandInput.Builder(sublocation, "subcmd").Build();

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(subcommand);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd << subcmd");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsJoinedSubcommand_ParsesSubcommand()
        {
            // arrange
            var sublocation = new SourceLocation(1, 7);
            var subcommand = new CommandInput.Builder(sublocation, "subcmd").Build();

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(subcommand);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd <<subcmd");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsSubcommandTokenMissingSubcommand_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd <<");

            // act, assert
            var ex = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Missing subcommand", ex.Message);
        }

        [Fact]
        public void Parse_InputContainsSubcommandWithParameters_ParsesSubcommand()
        {
            // arrange
            var sublocation = new SourceLocation(1, 9);
            var subcommandBuilder = new CommandInput.Builder(sublocation, "subcmd");
            subcommandBuilder.AddParameter(new ParameterNameCommandParameter("a"));
            subcommandBuilder.AddParameter(new Literal("name"));
            var subcommand = subcommandBuilder.Build();

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(subcommand);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd << (subcmd -a name)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsSubcommandMissingBlockEnd_ThrowException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd << (subcmd");

            // act, assert
            var ex = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Missing block end", ex.Message);
        }

        [Fact]
        public void Parse_InputContainsSubcommandMissingBlockStart_ThrowException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd << subcmd)");

            // act, assert
            var ex = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Missing block start", ex.Message);
        }

        [Fact]
        public void Parse_InputContainsSubcommandOnNewline_ParsesSubcommand()
        {
            // arrange
            var sublocation = new SourceLocation(2, 1);
            var subcommand = new CommandInput.Builder(sublocation, "subcmd").Build();

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(subcommand);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd << (\nsubcmd\n)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsListWithSubcommands_ParsesSubcommand()
        {
            // arrange
            var sublocation = new SourceLocation(1, 11);
            var subcommand = new CommandInput.Builder(sublocation, "subcmd").Build();

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new List(new ICommandParameter[] { new Literal("1"), subcommand }));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd [1 << subcmd]");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsListWithSubcommandWithParameters_ParsesSubcommand()
        {
            // arrange
            var sublocation = new SourceLocation(1, 12);
            var subcommandBuilder = new CommandInput.Builder(sublocation, "subcmd");
            subcommandBuilder.AddParameter(new Literal("foo"));
            var subcommand = subcommandBuilder.Build();

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(new List(new ICommandParameter[] { new Literal("1"), subcommand }));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd [1 << (subcmd foo)]");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsListWithSubcommandMissingCommand_ThrowException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd [1 << ]");

            // act, assert
            var ex = Assert.Throws<ParseException>(sutAction);
            Assert.Contains("Missing subcommand", ex.Message);
        }

        [Fact]
        public void Parse_InputContainsMultipleSubcommands_ParsesSubcommands()
        {
            // arrange
            var sublocation1 = new SourceLocation(1, 8);
            var subcommand1 = new CommandInput.Builder(sublocation1, "subcmd").Build();

            var sublocation2 = new SourceLocation(1, 18);
            var subcommand2 = new CommandInput.Builder(sublocation2, "subcmd").Build();

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(subcommand1);
            builder.AddParameter(subcommand2);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd << subcmd << subcmd");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsSubcommandAndBlock_ParsesSubcommandAndParameters()
        {
            // arrange
            var sublocation = new SourceLocation(1, 8);
            var subcommand = new CommandInput.Builder(sublocation, "subcmd").Build();

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(subcommand);
            builder.AddParameter(new Literal("foo bar"));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd << subcmd (foo bar)");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Fact]
        public void Parse_InputContainsNestedSubcommands_ParsesSubcommand()
        {
            // arrange
            var sublocationInner = new SourceLocation(1, 23);
            var subcommandBuilderInner = new CommandInput.Builder(sublocationInner, "subcmdi");
            subcommandBuilderInner.AddParameter(new Literal("foo"));
            var subCommandInner = subcommandBuilderInner.Build();

            var sublocationOuter = new SourceLocation(1, 9);
            var subcommandBuilderOuter = new CommandInput.Builder(sublocationOuter, "subcmdo");
            subcommandBuilderOuter.AddParameter(new ParameterNameCommandParameter("a"));
            subcommandBuilderOuter.AddParameter(subCommandInner);
            var subcommandOuter = subcommandBuilderOuter.Build();

            var location = new SourceLocation(1, 1);
            var builder = new CommandInput.Builder(location, "cmd");
            builder.AddParameter(subcommandOuter);
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse("cmd << (subcmdo -a <<(subcmdi foo))");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        private Parser CreateParser()
        {
            return new Parser(_nameValidator);
        }

        private CommandInput CreateCommandInput(int sourceLine, int sourceCharacter, string commandName)
        {
            var location = new SourceLocation(sourceLine, sourceCharacter);
            var builder = new CommandInput.Builder(location, commandName);
            return builder.Build();
        }
    }
}
