using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.UnitTests.SampleCommands;
using NSubstitute;
using Xunit;

namespace Chel.UnitTests
{
    public class SessionTests
    {
        [Fact]
        private void Ctor_ParserIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Session(null, Substitute.For<ICommandFactory>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("parser", ex.ParamName);
        }

        [Fact]
        private void Ctor_FactoryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Session(Substitute.For<IParser>(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandFactory", ex.ParamName);
        }

        [Fact]
        public void Execute_InputIsNull_DoNothing()
        {
            // arrange
            var sut = CreateSession();

            // act, assert
            sut.Execute(null, _ => {});
        }

        [Fact]
        private void Execute_ResultHandlerIsNull_ThrowsException()
        {
            // arrange
            var sut = CreateSession();
            Action sutAction = () => sut.Execute(null, null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("resultHandler", ex.ParamName);
        }

        [Fact]
        private void Execute_SingleCommandNotRegistered_PassesCommandNotFoundResultToHandler()
        {
            // arrange
            var sut = CreateSession();
            CommandResult executionResult = null;

            // act
            sut.Execute("sample", result => executionResult = result);

            // assert
            Assert.IsType<UnknownCommandResult>(executionResult);
        }

        [Fact]
        private void Execute_SingleCommand_PassesCommandResultToHandler()
        {
            // arrange
            var sut = CreateSession(typeof(SampleCommand));
            CommandResult executionResult = null;

            // act
            sut.Execute("sample", result => executionResult = result);
            
            // assert
            Assert.IsType<SuccessResult>(executionResult);
        }

        private Session CreateSession(params Type[] commandTypes)
        {
            var nameValidator = new NameValidator();
            var registry = new CommandRegistry(nameValidator);

            foreach(var commandType in commandTypes)
                registry.Register(commandType);

            var services = new CommandServices();

            var factory = new CommandFactory(registry, services);
            var parser = new Parser();

            return new Session(parser, factory);
        }
    }
}