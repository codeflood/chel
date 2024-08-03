using System;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Commands;
using Chel.UnitTests.SampleCommands;
using Xunit;

namespace Chel.UnitTests.Commands
{
    public class HelpTests
    {
        [Fact]
        public void Ctor_CommandRegistryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Help(null!, new ExecutionTargetIdentifierParser());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandRegistry", ex.ParamName);
        }

        [Fact]
        public void Ctor_ParserIsNull_ThrowsException()
        {
            // arrange
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);
            Action sutAction = () => new Help(registry, null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("executionTargetIdentifierParser", ex.ParamName);
        }

        [Fact]
        public void Execute_NoParametersSet_ListsAllCommands()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterCommand));

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            var commands = Assert.IsType<Literal>(result.Value);
            Assert.Contains("help", commands.Value);
            Assert.Contains("num", commands.Value);
        }

        [Fact]
        public void Execute_NoParametersSet_ListsAllModules()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterModuleCommand), typeof(DuplicateSampleCommandDifferentModule));

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            var commands = Assert.IsType<Literal>(result.Value);
            Assert.Contains("help", commands.Value);
            Assert.Contains("diffmod, mod", commands.Value);
        }

        [Fact]
        public void Execute_SameModuleDifferentCasing_ShowsDeduplicatedModules()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(AnotherCommandDifferentModuleUppercase), typeof(NumberedParameterModuleCommand), typeof(DuplicateSampleCommandDifferentModule));

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            var commands = Assert.IsType<Literal>(result.Value);
            Assert.Contains("help", commands.Value);
            Assert.Contains("diffmod, mod", commands.Value);
        }

        [Fact]
        public void Execute_NoParametersSet_DisplaysDescriptions()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterCommand));

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            var commands = Assert.IsType<Literal>(result.Value);
            Assert.Contains("Lists available commands and displays help for commands.", commands.Value);
            Assert.Contains("A sample command with numbered parameters.", commands.Value);
        }

        [Fact]
        public void Execute_NoParametersSet_ListsCommandsInAlphabeticalOrder()
        {
            // arrange
            var sut = CreateSut(typeof(Var), typeof(NumberedParameterCommand), typeof(Help));

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            var commands = Assert.IsType<Literal>(result.Value);
            var helpIndex = commands.Value.IndexOf("help");
            var numIndex = commands.Value.IndexOf("num");
            var varIndex = commands.Value.IndexOf("var");
            Assert.True(helpIndex < numIndex);
            Assert.True(numIndex < varIndex);
        }

        [Fact]
        public void Execute_NoDescriptionOnCommand_DoesNotError()
        {
            // arrange
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);
            registry.Register(typeof(SampleCommand2));

            var sut = new Help(registry, new ExecutionTargetIdentifierParser());

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Contains("sample2", result.Value.ToString());
        }

        [Fact]
        public void Execute_CommandIdentifierSet_DisplaysDetailsForCommand()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterCommand));
            sut.CommandIdentifier = "num";

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Contains("usage: num [param1] [param2]", result.Value.ToString());
            Assert.Contains("A sample command with numbered parameters.", result.Value.ToString());
            Assert.Matches(@"param1\s+The first parameter", result.Value.ToString());
            Assert.Matches(@"param2\s+The second parameter", result.Value.ToString());
        }

        [Theory]
        [InlineData("mod:")]
        [InlineData("MOD:")]
        public void Execute_CommandIdentifierHasExistingModule_ListsCommandInModuleOnly(string commandIdentifier)
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NamedParameterCommand), typeof(NumberedParameterModuleCommand));
            sut.CommandIdentifier = commandIdentifier;

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            var commands = Assert.IsType<Literal>(result.Value);
            var commandLines = ((Literal)result.Value).Value.Split(Environment.NewLine);

            Assert.Contains(commandLines, x => x.StartsWith("num"));
            Assert.DoesNotContain(commandLines, x => x.StartsWith("nam"));
        }

        [Fact]
        public void Execute_CommandIdentifierHasUnknownModule_ReturnsError()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NamedParameterCommand), typeof(NumberedParameterModuleCommand));
            sut.CommandIdentifier = "bad:";

            // act
            var result = (FailureResult)sut.Execute();

            // assert
        Assert.Equal("Cannot display help for unknown module 'bad'", result.Message);
        }

        [Fact]
        public void Execute_CommandIdentifierSetWithModuleAndCommand_DisplaysDetailsForCommand()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterModuleCommand), typeof(NamedParameterCommand));
            sut.CommandIdentifier = "mod:num";

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Contains("usage: mod:num [param1] [param2]", result.Value.ToString());
            Assert.Contains("A sample command with numbered parameters.", result.Value.ToString());
            Assert.Matches(@"param1\s+The first parameter", result.Value.ToString());
            Assert.Matches(@"param2\s+The second parameter", result.Value.ToString());
        }

        [Fact]
        public void Execute_CommandNameParameterSetUppercase_DisplaysDetailsForCommand()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterCommand));
            sut.CommandIdentifier = "NUM";

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Contains("usage: num [param1] [param2]", result.Value.ToString());
            Assert.Contains("A sample command with numbered parameters.", result.Value.ToString());
            Assert.Matches(@"param1\s+The first parameter", result.Value.ToString());
            Assert.Matches(@"param2\s+The second parameter", result.Value.ToString());
        }

        [Fact]
        public void Execute_CommandNameParameterSetCommandNotRegistered_ReturnsFailureResult()
        {
            // arrange
            var sut = CreateSut(typeof(Help), typeof(NumberedParameterCommand));
            sut.CommandIdentifier = "boo";

            // act
            var result = (FailureResult)sut.Execute();

            // assert
            Assert.Equal("Cannot display help for unknown command 'boo'", result.Message);
        }

        [Fact]
        public void Execute_CommandIncludesRequiredParameter_RequiredParametersShownInOutput()
        {
            // arrange
            var sut = CreateSut(typeof(RequiredParameterCommand));
            sut.CommandIdentifier = "command";

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Contains("usage: command param", result.Value.ToString());
            Assert.Matches(@"param\s+Required. The first parameter", result.Value.ToString());
        }

        [Fact]
        public void Execute_CommandIncludesRequiredNamedParameter_NamedParametersShownInUsage()
        {
            // arrange
            var sut = CreateSut(typeof(RequiredNamedParameterCommand));
            sut.CommandIdentifier = "command";

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Contains("usage: command -param <value>", result.Value.ToString());
            Assert.Matches(@"-param <value>\s+Required. A required parameter.", result.Value.ToString());
        }

        [Fact]
        public void Execute_CommandIncludesNamedParameter_NamedParametersShownInUsage()
        {
            // arrange
            var sut = CreateSut(typeof(NamedParameterCommand));
            sut.CommandIdentifier = "nam";

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Contains("usage: nam [-param1 <value1>] [-param2 <value2>]", result.Value.ToString());
            Assert.Matches(@"-param1 <value1>\s+The param1 parameter.", result.Value.ToString());
            Assert.Matches(@"-param2 <value2>\s+The param2 parameter.", result.Value.ToString());
        }

        [Fact]
        public void Execute_CommandIncludesFlagParameter_FlagParametersShownInUsage()
        {
            // arrange
            var sut = CreateSut(typeof(FlagParameterCommand));
            sut.CommandIdentifier = "command";

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Contains("usage: command [-p1] [-p2]", result.Value.ToString());
            Assert.Matches(@"-p1\s+The p1 parameter.", result.Value.ToString());
            Assert.Matches(@"-p2\s+The p2 parameter.", result.Value.ToString());
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

            var executionTargetIdentifierParser = new ExecutionTargetIdentifierParser();
            return new Help(registry, executionTargetIdentifierParser);
        }
    }
}