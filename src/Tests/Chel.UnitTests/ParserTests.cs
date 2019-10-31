using System;
using System.Collections.Generic;
using System.Linq;
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

            // todo: param with spaces
        }

        private CommandInput CreateCommandInput(int sourceLine, string commandName)
        {
            var builder = new CommandInput.Builder(sourceLine, commandName);
            return builder.Build();
        }
    }
}
