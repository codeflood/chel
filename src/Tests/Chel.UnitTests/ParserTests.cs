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
        [MemberData(nameof(InputWithNumberedParametersDataSource))]
        public void Parse_InputWithNumberedParameters_ParametersParsedInOrder(string input, CommandInput[] expected)
        {
            // arrange
            var sut = new Parser();

            // act
            var result = sut.Parse(input);

            // assert
            Assert.Equal(expected, result.ToArray());
        }

        public static IEnumerable<object[]> InputWithNumberedParametersDataSource()
        {
            var builder1 = new CommandInput.Builder(1, "command");
            builder1.AddNumberedParameter("param");
            var commandInput1 = builder1.Build();

            yield return new object[]{ "command param", new[] { commandInput1 } };
            yield return new object[]{ "command   param", new[] { commandInput1 } };
            yield return new object[]{ "command\tparam", new[] { commandInput1 } };
            yield return new object[]{ "command  \t  param  \t", new[] { commandInput1 } };

            var builder2 = new CommandInput.Builder(1, "command");
            builder2.AddNumberedParameter("param1");
            builder2.AddNumberedParameter("param2");
            var commandInput2 = builder2.Build();

            yield return new object[]{ "command param1 param2", new[] { commandInput2 } };
            yield return new object[]{ "command  \t param1   param2", new[] { commandInput2 } };

            var builder3 = new CommandInput.Builder(2, "command");
            builder3.AddNumberedParameter("param1");
            builder3.AddNumberedParameter("param2");
            var commandInput3 = builder3.Build();

            yield return new object[]{ "command param\ncommand param1\tparam2", new[] { commandInput1, commandInput3 } };
            yield return new object[]{ "command param  \n  command  param1    param2", new[] { commandInput1, commandInput3 } };
            yield return new object[]{ "command param  #comment\ncommand param1\tparam2", new[] { commandInput1, commandInput3 } };

            var builder4 = new CommandInput.Builder(1, "command");
            builder4.AddNumberedParameter("pa ram");
            var commandInput4 = builder4.Build();

            yield return new object[]{ "command (pa ram)", new[] { commandInput4 } };

            var builder5 = new CommandInput.Builder(1, "command");
            builder5.AddNumberedParameter(" param  param ");
            var commandInput5 = builder5.Build();

            yield return new object[]{ "command ( param  param )", new[] { commandInput5 } };

            var builder6 = new CommandInput.Builder(1, "command");
            builder6.AddNumberedParameter("\n\n");
            var commandInput6 = builder6.Build();

            yield return new object[]{ "command (\n\n)", new[] { commandInput6 } };

            var builder7 = new CommandInput.Builder(1, "command");
            builder7.AddNumberedParameter("(param)");
            var commandInput7 = builder7.Build();

            yield return new object[]{ @"command \(param\)", new[]{ commandInput7 } };

            var builder8 = new CommandInput.Builder(1, "command");
            builder8.AddNumberedParameter("#");
            var commandInput8 = builder8.Build();

            yield return new object[]{ @"command \#", new[]{ commandInput8 } };

            var builder9 = new CommandInput.Builder(1, "command");
            builder9.AddNumberedParameter("#");
            builder9.AddNumberedParameter("param");
            var commandInput9 = builder9.Build();

            yield return new object[]{ @"command \# param", new[]{ commandInput9 } };

            var builder10 = new CommandInput.Builder(1, "command");
            builder10.AddNumberedParameter(" param  param ");
            builder10.AddNumberedParameter("param");
            var commandInput10 = builder10.Build();

            yield return new object[]{ "command ( param  param )  param ", new[] { commandInput10 } };

            var builder11 = new CommandInput.Builder(1, "command");
            builder11.AddNumberedParameter("param\nparam");
            var commandInput11 = builder11.Build();

            yield return new object[]{ "command (param\nparam) ", new[] { commandInput11 } };
        }

        [Fact]
        public void Parse_UnbalancedParenthesis_ThrowsException()
        {
            // arrange
            var sut = new Parser();
            Action sutAction = () => sut.Parse("command (param");

            // act, assert
            var exception = Assert.Throws<ParserException>(sutAction);
            Assert.Equal("Missing )", exception.Message);
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
