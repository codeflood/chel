using System;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Results;
using Chel.Abstractions.Variables;
using Chel.Exceptions;
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
            Action sutAction = () => new Session(
                    null,
                    Substitute.For<ICommandFactory>(),
                    Substitute.For<ICommandParameterBinder>()
                );

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("parser", ex.ParamName);
        }

        [Fact]
        private void Ctor_FactoryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Session(
                    Substitute.For<IParser>(),
                    null,
                    Substitute.For<ICommandParameterBinder>()
                );

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandFactory", ex.ParamName);
        }

        [Fact]
        public void Ctor_BinderIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Session(
                    Substitute.For<IParser>(),
                    Substitute.For<ICommandFactory>(),
                    null
                );

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("parameterBinder", ex.ParamName);
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
            Assert.IsType<FailureResult>(executionResult);
            var failureResult = executionResult as FailureResult;
            Assert.Equal(1, failureResult.SourceLine);
        }

        [Fact]
        private void Execute_ErrorOnLine2_ReportsFailureOnLine2()
        {
            // arrange
            var sut = CreateSession(typeof(SampleCommand));
            CommandResult executionResult = null;

            // act
            sut.Execute("sample\nboo", result => executionResult = result);

            // assert
            Assert.IsType<FailureResult>(executionResult);
            var failureResult = executionResult as FailureResult;
            Assert.Equal(2, failureResult.SourceLine);
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

        [Fact]
        public void Execute_ParameterProvided_ParameterSetOnCommand()
        {
            // arrange
            var sut = CreateSession(typeof(NumberedParameterCommand));
            ValueResult executionResult = null;

            // act
            sut.Execute("num p1 p2", result => executionResult = result as ValueResult);
            
            // assert
            Assert.Equal("p1 p2", executionResult.Value);
        }

        [Fact]
        public void Execute_ParameterBindingHasErrors_FailureResultReturned()
        {
            // arrange
            var sut = CreateSession(typeof(NumberedParameterCommand));
            FailureResult executionResult = null;

            // act
            sut.Execute("num p1 p2 p3", result => executionResult = result as FailureResult);
            
            // assert
            Assert.Equal("ERROR (Line 1): Unexpected numbered parameter 'p3'", executionResult.ToString());
        }

        [Fact]
        public void Execute_ParserThrowsException_FailureResultReturned()
        {
            // arrange
            var parser = Substitute.For<IParser>();
            var location = new SourceLocation(1, 1);
            parser.Parse(Arg.Any<string>()).Returns(x => { throw new ParserException(location, "message"); });

            var factory = Substitute.For<ICommandFactory>();
            var binder = Substitute.For<ICommandParameterBinder>();

            var sut = new Session(parser, factory, binder);
            FailureResult executionResult = null;

            // act
            sut.Execute("command", result => executionResult = result as FailureResult);

            // assert
            Assert.Equal(1, executionResult.SourceLine);
        }

        [Fact]
        public void Execute_FactoryThrowsException_FailureResultReturned()
        {
            // arrange
            var commandBuilder = new CommandInput.Builder(1, "command");
            var commandInput = commandBuilder.Build();

            var parser = Substitute.For<IParser>();
            parser.Parse(Arg.Any<string>()).Returns(new[]{ commandInput });

            var factory = Substitute.For<ICommandFactory>();
            factory.Create(Arg.Any<CommandInput>()).Returns(x => { throw new Exception(); });

            var binder = Substitute.For<ICommandParameterBinder>();

            var sut = new Session(parser, factory, binder);
            FailureResult executionResult = null;

            // act
            sut.Execute("command", result => executionResult = result as FailureResult);

            // assert
            Assert.Equal(1, executionResult.SourceLine);
        }

        [Fact]
        public void Execute_CommandThrowsException_FailureResultReturned()
        {
            // arrange
            var sut = CreateSession(typeof(ExceptionCommand));
            FailureResult executionResult = null;

            // act
            sut.Execute("ex", result => executionResult = result as FailureResult);

            // assert
            Assert.Equal(1, executionResult.SourceLine);
        }

        [Fact]
        public void Execute_ParameterIsUnsetVariable_ReturnsFailure()
        {
            // arrange
            var sut = CreateSession(typeof(NamedParameterCommand));
            FailureResult executionResult = null;

            // act
            sut.Execute("nam -param1 $foo$", result => executionResult = result as FailureResult);

            // assert
            Assert.Contains("Variable $foo$ is not set", executionResult.Messages);
        }

        [Fact]
        public void Execute_ParameterIsSetVariable_PerformsSubstitution()
        {
            // arrange
            var sut = CreateSession(
                sessionObjectsConfigurator: x => {
                    x.Register<VariableCollection>();
                    ((VariableCollection)x.Resolve(typeof(VariableCollection))).Set(new ValueVariable("foo", "lorem"));
                },
                typeof(NamedParameterCommand));

            CommandResult executionResult = null;

            // act
            sut.Execute("nam -param1 $foo$ -param2 $foo$", result => executionResult = result);

            // assert
            Assert.IsType<ValueResult>(executionResult);
            Assert.Equal("lorem lorem", (executionResult as ValueResult).Value);
        }

        private Session CreateSession(params Type[] commandTypes)
        {
            return CreateSession(null, commandTypes);
        }

        private Session CreateSession(
            Action<ScopedObjectRegistry> sessionObjectsConfigurator = null,
            params Type[] commandTypes)
        {
            var parser = new Parser();
            var nameValidator = new NameValidator();
            var descriptorGenerator = new CommandAttributeInspector();
            var registry = new CommandRegistry(nameValidator, descriptorGenerator);

            foreach(var commandType in commandTypes)
                registry.Register(commandType);

            var services = new CommandServices();
            var scopedObjects = new ScopedObjectRegistry();
            scopedObjects.Register<VariableCollection>();
            sessionObjectsConfigurator?.Invoke(scopedObjects);

            var factory = new CommandFactory(registry, services, scopedObjects);

            var variables = scopedObjects.Resolve(typeof(VariableCollection)) as VariableCollection;
            var replacer = new VariableReplacer();
            var binder = new CommandParameterBinder(registry, replacer, variables);

            return new Session(parser, factory, binder);
        }
    }
}