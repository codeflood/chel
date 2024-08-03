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
            Action sutAction = () => new Parser(null!);

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
        public void Parse_EmptyInput_ReturnsEmptyCommandInputList(string? input)
        {
            // arrange
            var sut = CreateParser();

            // act
            var result = sut.Parse(input!);

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_SingleCommand_ReturnsCommandInput()
        {
            // arrange
            var sut = CreateParser();
            var expected = new[] { CreateCommandInputBuilder(1, 1, "command").Build() };

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
            var expected = new[] { CreateCommandInputBuilder(1, 1, "comm-and").Build() };

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
            var expected = new[]
            {
                CreateCommandInputBuilder(sourceLine1, sourceCharacter1, "command").Build(),
                CreateCommandInputBuilder(sourceLine2, sourceCharacter2, "command").Build()
            };

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
            var expected = new[] { CreateCommandInputBuilder(sourceLine, sourceCharacter, "command").Build() };

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Parse_SingleParameter_ParameterParsed_DataSource))]
        public void Parse_SingleParameter_ParameterParsed(string input, SourceLocation parameterLocation)
        {
            // arrange
            var sut = CreateParser();
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(parameterLocation, new Literal("param")));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        public static IEnumerable<object[]> Parse_SingleParameter_ParameterParsed_DataSource()
        {
            yield return new object[] { "command param", new SourceLocation(1, 9) };
            yield return new object[] { "command   param  ", new SourceLocation(1, 11) };
            yield return new object[] { "command\tparam", new SourceLocation(1, 9) };
            yield return new object[] { "command \t param \t", new SourceLocation(1, 11) };
        }

        [Theory]
        [InlineData("command param1 param2", 9, 16)]
        [InlineData("command\tparam1\tparam2  ", 9, 16)]
        [InlineData("command  \t  param1  \t  param2  \t", 13, 24)]
        public void Parse_TwoParameters_ParametersParsed(string input, int p1char, int p2char)
        {
            // arrange
            var sut = CreateParser();
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, p1char), new Literal("param1")));
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, p2char), new Literal("param2")));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Theory]
        [InlineData("command param1 param2\ncommand param1 param2", 9, 16, 9, 16)]
        [InlineData("command\tparam1\tparam2  \ncommand\tparam1\tparam2  ", 9, 16, 9, 16)]
        [InlineData("command  \t  param1  \t  param2  \t\ncommand  \t  param1  \t  param2  \t", 13, 24, 13, 24)]
        public void Parse_TwoCommandsTwoParameters_ParsedCommandsWithParameters(string input, int c1p1char, int c1p2char, int c2p1char, int c2p2char)
        {
            // arrange
            var sut = CreateParser();
            
            var builder1 = CreateCommandInputBuilder(1, 1, "command");
            builder1.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, c1p1char), new Literal("param1")));
            builder1.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, c1p2char), new Literal("param2")));
            var expectedCommand1 = builder1.Build();

            var builder2 = CreateCommandInputBuilder(2, 1, "command");
            builder2.AddParameter(new SourceValueCommandParameter(new SourceLocation(2, c2p1char), new Literal("param1")));
            builder2.AddParameter(new SourceValueCommandParameter(new SourceLocation(2, c2p2char), new Literal("param2")));
            var expectedCommand2 = builder2.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(new List<CommandInput>(new[] { expectedCommand1, expectedCommand2 }), result);
        }

        [Theory]
        [InlineData("command -name value", 9, 15)]
        [InlineData("command  -name\tvalue", 10, 16)]
        public void Parse_NamedParameters_ParameterParsed(string input, int p1char, int p2char)
        {
            // arrange
            var sut = CreateParser();
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new ParameterNameCommandParameter(new SourceLocation(1, p1char), "name"));
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, p2char), new Literal("value")));
            var expectedCommand = builder.Build();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Theory]
        [InlineData("command -name value -name2 value2", 9, 15, 21, 28)]
        [InlineData("command  -name\tvalue   -name2\tvalue2", 10, 16, 24, 31)]
        public void Parse_MultipleNamedParameters_ParameterParsed(string input, int p1char, int p2char, int p3char, int p4char)
        {
            // arrange
            var sut = CreateParser();
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new ParameterNameCommandParameter(new SourceLocation(1, p1char), "name"));
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, p2char), new Literal("value")));
            builder.AddParameter(new ParameterNameCommandParameter(new SourceLocation(1, p3char), "name2"));
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, p4char), new Literal("value2")));
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
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 9), new Literal("pa ram")));
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
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 9), new Literal("\n\n")));
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
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 10), new Literal("(param)")));
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
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 10), new Literal(@"\")));
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
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 10), new Literal("#")));
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 12), new Literal("param")));
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
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 9), new Literal(" param  param ")));
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 27), new Literal("param")));
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
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 9), new Literal("param\nparam")));
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
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 9), new Literal("(param)")));
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
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 9), new Literal("\nic  (pa ram)\nic (pa ram)")));
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
            var builder = CreateCommandInputBuilder(1, 1, "command");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 9), new Literal("pa ram")));
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 18), new Literal("pa ram")));
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
            Assert.Equal(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.MissingBlockEnd), exception.Message);
        }

        [Fact]
        public void Parse_MissingOpeningBracket_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("command param )");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.MissingBlockStart), exception.Message);
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), new VariableReference("var")));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 6), new Literal("$var$")));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5),  new VariableReference("var", new[] { "1" })));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), new VariableReference("var", new[] { "1", "2" })));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new ParameterNameCommandParameter(new SourceLocation(1, 5), "par"));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 6), new Literal("-par")));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), new Literal("p-ar")));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), new Literal("p -ar")));
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
            var expectedList = new List(new[]
            {
                new SourceValueCommandParameter(new SourceLocation(1, 6), new Literal("a")),
                new SourceValueCommandParameter(new SourceLocation(1, 8), new VariableReference("b"))
            });

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), expectedList));
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
            Assert.Equal(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.MissingListEnd), exception.Message);
        }

        [Fact]
        public void Parse_InputMissingListStart_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd a]");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.MissingListStart), exception.Message);
        }

        [Fact]
        public void Parse_InputContainsEscapedListParameter_ParsesAsLiteral()
        {
            // arrange
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 6), new Literal("[a]")));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), new Literal("[a b]")));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");

            var map1Entries = new Dictionary<string, ICommandParameter> { { "a", new SourceValueCommandParameter(new SourceLocation(1, 9), new Literal("b")) }};
            var map2Entries = new Dictionary<string, ICommandParameter> { { "c", new SourceValueCommandParameter(new SourceLocation(1, 20), new Literal("d")) }};

            builder.AddParameter(
                new SourceValueCommandParameter(new SourceLocation(1, 5),
                    new List(new[] {
                        new SourceValueCommandParameter(new SourceLocation(1, 6), new Map(map1Entries)),
                        new SourceValueCommandParameter(new SourceLocation(1, 14), new Map(map2Entries))
                    }
                ))
            );
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(@"cmd [{a:b}   { c : d } ]");

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        [Theory]
        [MemberData(nameof(Parse_InputContainsMapParameter_ParsesMap_DataSource))]
        public void Parse_InputContainsMapParameter_ParsesMap(string input, SourceLocation valueLocation)
        {
            // arrange
            var expectedMap = new Map(new Dictionary<string, ICommandParameter>
            {
                { "a", new SourceValueCommandParameter(valueLocation, new Literal("b")) }
            });

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), expectedMap));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        public static IEnumerable<object[]> Parse_InputContainsMapParameter_ParsesMap_DataSource()
        {
            yield return new object[] { "cmd {a: b}", new SourceLocation(1, 9) };
            yield return new object[] { "cmd { a: b }", new SourceLocation(1, 10) };
            yield return new object[] { "cmd {a:b}", new SourceLocation(1, 8) };
            yield return new object[] { "cmd {     a        :      b        }", new SourceLocation(1, 27) };
            yield return new object[] { "cmd {\n  a:  b  \n}", new SourceLocation(2, 7) };
        }

        [Theory]
        [MemberData(nameof(Parse_InputContainsMapParameterWithVariableReference_ParsesMap_DataSource))]
        public void Parse_InputContainsMapParameterWithVariableReference_ParsesMap(string input, SourceLocation valueLocation)
        {
            // arrange
            var expectedMap = new Map(new Dictionary<string, ICommandParameter>
            {
                { "a", new SourceValueCommandParameter(valueLocation, new VariableReference("b")) }
            });

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), expectedMap));
            var expectedCommand = builder.Build();

            var sut = CreateParser();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expectedCommand, result[0]);
        }

        public static IEnumerable<object[]> Parse_InputContainsMapParameterWithVariableReference_ParsesMap_DataSource()
        {
            yield return new object[] { "cmd {a: $b$}", new SourceLocation(1, 9) };
            yield return new object[] { "cmd {a:$b$}", new SourceLocation(1, 8)};
            yield return new object[] { "cmd {     a        :      $b$        }", new SourceLocation(1, 27) };
            yield return new object[] { "cmd {\n  a:  $b$  \n}", new SourceLocation(2, 7) };
        }

        [Fact]
        public void Parse_InputContainsMapParameterWithManyEntries_ParsesMap()
        {
            // arrange
            var expectedMap = new Map(new Dictionary<string, ICommandParameter>
            {
                { "foo", new SourceValueCommandParameter(new SourceLocation(1, 11), new Literal("bar")) },
                { "num", new SourceValueCommandParameter(new SourceLocation(1, 21), new Literal("12")) }
            });

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), expectedMap));
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
                { "foo", new SourceValueCommandParameter(new SourceLocation(1, 11), new Literal("bar baz")) },
                { "num", new SourceValueCommandParameter(new SourceLocation(1, 27),new Literal("12")) }
            });

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), expectedMap));
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
                { "foo", new SourceValueCommandParameter(new SourceLocation(2, 22), new Literal("bar baz")) },
                { "num", new SourceValueCommandParameter(new SourceLocation(3, 23), new Literal("12")) }
            });

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), expectedMap));
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
                { "the-key", new SourceValueCommandParameter(new SourceLocation(1, 16), new Literal("the-value")) }
            });

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), expectedMap));
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
            Assert.Equal(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.MissingMapEnd), exception.Message);
        }

        [Fact]
        public void Parse_InputMissingMapStart_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd a:b}");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.MissingMapStart), exception.Message);
        }

        [Fact]
        public void Parse_InputContainsEscapedMapParameter_ParsesAsLiteral()
        {
            // arrange
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 6), new Literal("{a:b}")));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5),new Literal("{a: b}")));
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
            var builder = CreateCommandInputBuilder(1, 1, "cmd");

            var expectedMap1 = new Map(new Dictionary<string, ICommandParameter>
            {
                { "a", new SourceValueCommandParameter(new SourceLocation(1, 9), new List(new[]
                {
                    new SourceValueCommandParameter(new SourceLocation(1, 10), new Literal("1")),
                    new SourceValueCommandParameter(new SourceLocation(1, 12), new Literal("2"))
                }))},
                { "b", new SourceValueCommandParameter(new SourceLocation(1, 20), new List(new[]
                {
                    new SourceValueCommandParameter(new SourceLocation(1, 22), new Literal("3")),
                    new SourceValueCommandParameter(new SourceLocation(1, 24), new Literal("4"))
                }))}
            });

            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), expectedMap1));
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
            Assert.Equal(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.MissingMapEntryName), exception.Message);
        }

        [Fact]
        public void Parse_MapMissingNameDelimiter_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd { a b }");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.MissingMapEntryName), exception.Message);
        }

        [Fact]
        public void Parse_MapMissingValue_ThrowsException()
        {
            // arrange
            var sut = CreateParser();
            Action sutAction = () => sut.Parse("cmd { a: }");

            // act, assert
            var exception = Assert.Throws<ParseException>(sutAction);
            Assert.Equal(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.MissingMapEntryValue), exception.Message);
        }

        [Fact]
        public void Parse_InputContainsSeparatedSubcommand_ParsesSubcommand()
        {
            // arrange
            var subcommand = CreateCommandInputBuilder(1, 8, "subcmd").Build();

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
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
            var subcommand = CreateCommandInputBuilder(1, 7, "subcmd").Build();

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
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
            var subcommandBuilder = CreateCommandInputBuilder(1, 9, "subcmd");
            subcommandBuilder.AddParameter(new ParameterNameCommandParameter(new SourceLocation(1, 16), "a"));
            subcommandBuilder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 19), new Literal("name")));
            var subcommand = subcommandBuilder.Build();

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
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
            var subcommand = CreateCommandInputBuilder(2, 1, "subcmd").Build();

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
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
            var subcommand = CreateCommandInputBuilder(1, 11, "subcmd").Build();

            var expectedList = new List(new ICommandParameter[]
            {
                new SourceValueCommandParameter(new SourceLocation(1, 6), new Literal("1")),
                subcommand
            });

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), expectedList));
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
            var subcommandBuilder = CreateCommandInputBuilder(1, 12, "subcmd");
            subcommandBuilder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 19), new Literal("foo")));
            var subcommand = subcommandBuilder.Build();

            var expectedList = new List(new ICommandParameter[]
            {
                new SourceValueCommandParameter(new SourceLocation(1, 6), new Literal("1")),
                subcommand
            });

            var builder = CreateCommandInputBuilder(1, 1, "cmd");

            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 5), expectedList));
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
            var subcommand1 = CreateCommandInputBuilder(1, 8, "subcmd").Build();
            var subcommand2 = CreateCommandInputBuilder(1, 18, "subcmd").Build();

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
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
            var subcommand = CreateCommandInputBuilder(1, 8, "subcmd").Build();

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
            builder.AddParameter(subcommand);
            builder.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 15), new Literal("foo bar")));
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
            var subcommandBuilderInner = CreateCommandInputBuilder(1, 23, "subcmdi");
            subcommandBuilderInner.AddParameter(new SourceValueCommandParameter(new SourceLocation(1, 31), new Literal("foo")));
            var subCommandInner = subcommandBuilderInner.Build();

            var subcommandBuilderOuter = CreateCommandInputBuilder(1, 9, "subcmdo");
            subcommandBuilderOuter.AddParameter(new ParameterNameCommandParameter(new SourceLocation(1, 17), "a"));
            subcommandBuilderOuter.AddParameter(subCommandInner);
            var subcommandOuter = subcommandBuilderOuter.Build();

            var builder = CreateCommandInputBuilder(1, 1, "cmd");
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

        private CommandInput.Builder CreateCommandInputBuilder(int sourceLine, int sourceCharacter, string commandName)
        {
            var location = new SourceLocation(sourceLine, sourceCharacter);
            var builder = new CommandInput.Builder(location, new ExecutionTargetIdentifier(null, commandName));
            return builder;
        }
    }
}
