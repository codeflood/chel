using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Commands;
using Chel.UnitTests.SampleCommands;
using NSubstitute;
using Xunit;

namespace Chel.UnitTests.Commands
{
    public class HelpTests
    {
        [Fact]
        public void Ctor_CommandRegistryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Help(null, Substitute.For<IPhraseDictionary>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandRegistry", ex.ParamName);
        }

        [Fact]
        public void Ctor_PhraseDictionaryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Help(Substitute.For<ICommandRegistry>(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("phraseDictionary", ex.ParamName);
        }

        [Fact]
        public void Execute_NoParametersSet_ListsAllCommands()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterCommand));

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("help", result.Value);
            Assert.Contains("num", result.Value);
        }

        [Fact]
        public void Execute_NoParametersSet_DisplaysDescriptions()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterCommand));

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("Lists available commands and displays help for commands.", result.Value);
            Assert.Contains("A sample command with numbered parameters.", result.Value);
        }

        [Fact]
        public void Execute_NoDescriptionOnCommand_DoesNotError()
        {
            // arrange
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);
            registry.Register(typeof(SampleCommand2));

            var sut = new Help(registry, new PhraseDictionary());

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("sample2", result.Value);
        }

        [Fact]
        public void Execute_CommandNameParameterSet_DisplaysDetailsForCommand()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterCommand));
            sut.CommandName = "num";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("usage: num [param1] [param2]", result.Value);
            Assert.Contains("A sample command with numbered parameters.", result.Value);
            Assert.Matches(@"param1\s+The first parameter", result.Value);
            Assert.Matches(@"param2\s+The second parameter", result.Value);
        }

        [Fact]
        public void Execute_CommandNameParameterSetUppercase_DisplaysDetailsForCommand()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterCommand));
            sut.CommandName = "NUM";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("usage: num [param1] [param2]", result.Value);
            Assert.Contains("A sample command with numbered parameters.", result.Value);
            Assert.Matches(@"param1\s+The first parameter", result.Value);
            Assert.Matches(@"param2\s+The second parameter", result.Value);
        }

        [Fact]
        public void Execute_CommandNameParameterSetCommandNotRegistered_ReturnsFailureResult()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterCommand));
            sut.CommandName = "boo";

            // act
            var result = sut.Execute() as FailureResult;

            // assert
            Assert.Equal(new[]{ "Cannot display help for unknown command 'boo'" }, result.Messages);
        }

        [Fact]
        public void Execute_CommandIncludesRequiredParameter_RequiredParametersShownInOutput()
        {
            // arrange
            var sut = CreateSut(typeof(RequiredParameterCommand));
            sut.CommandName = "command";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("usage: command param", result.Value);
            Assert.Matches(@"param\s+Required. The first parameter", result.Value);
        }

        [Fact]
        public void Execute_CommandIncludesRequiredNamedParameter_NamedParametersShownInUsage()
        {
            // arrange
            var sut = CreateSut(typeof(RequiredNamedParameterCommand));
            sut.CommandName = "command";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("usage: command -param <value>", result.Value);
            Assert.Matches(@"-param <value>\s+Required. A required parameter.", result.Value);
        }

        [Fact]
        public void Execute_CommandIncludesNamedParameter_NamedParametersShownInUsage()
        {
            // arrange
            var sut = CreateSut(typeof(NamedParameterCommand));
            sut.CommandName = "nam";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("usage: nam [-param1 <value1>] [-param2 <value2>]", result.Value);
            Assert.Matches(@"-param1 <value1>\s+The param1 parameter.", result.Value);
            Assert.Matches(@"-param2 <value2>\s+The param2 parameter.", result.Value);
        }

        [Fact]
        public void Execute_CommandIncludesFlagParameter_FlagParametersShownInUsage()
        {
            // arrange
            var sut = CreateSut(typeof(FlagParameterCommand));
            sut.CommandName = "command";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("usage: command [-p1] [-p2]", result.Value);
            Assert.Matches(@"-p1\s+The p1 parameter.", result.Value);
            Assert.Matches(@"-p2\s+The p2 parameter.", result.Value);
        }

        private Help CreateSut(params Type[] commandTypes)
        {
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);

            foreach(var type in commandTypes)
            {
                registry.Register(type);
            }

            return new Help(registry, new PhraseDictionary());
        }
    }
}