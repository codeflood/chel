using System;
using Chel.Abstractions.Results;
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
            Action sutAction = () => new Help(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandRegistry", ex.ParamName);
        }

        [Fact]
        public void Execute_NoParametersSet_ListsAllCommands()
        {
            // arrange
            var sut = CreateSut();

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
            var sut = CreateSut();

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

            var sut = new Help(registry);

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("sample2", result.Value);
        }

        [Fact]
        public void Execute_CommandNameParameterSet_DisplaysDetailsForCommand()
        {
            // arrange
            var sut = CreateSut();
            sut.CommandName = "num";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("num param1 param2", result.Value);
            Assert.Contains("A sample command with numbered parameters.", result.Value);
            Assert.Matches(@"param1\s+The first parameter", result.Value);
            Assert.Matches(@"param2\s+The second parameter", result.Value);
        }

        [Fact]
        public void Execute_CommandNameParameterSetUppercase_DisplaysDetailsForCommand()
        {
            // arrange
            var sut = CreateSut();
            sut.CommandName = "NUM";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Contains("num param1 param2", result.Value);
            Assert.Contains("A sample command with numbered parameters.", result.Value);
            Assert.Matches(@"param1\s+The first parameter", result.Value);
            Assert.Matches(@"param2\s+The second parameter", result.Value);
        }

        [Fact]
        public void Execute_CommandNameParameterSetCommandNotRegistered_ReturnsFailureResult()
        {
            // arrange
            var sut = CreateSut();
            sut.CommandName = "boo";

            // act
            var result = sut.Execute() as FailureResult;

            // assert
            Assert.Equal(new[]{ Texts.UnknownCommand }, result.Messages);
        }

        private Help CreateSut()
        {
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);
            registry.Register(typeof(Help));
            registry.Register(typeof(NumberedParameterCommand));

            return new Help(registry);
        }
    }
}