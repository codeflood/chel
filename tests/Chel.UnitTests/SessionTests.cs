using System;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Abstractions.UnitTests.SampleCommands;
using Chel.Abstractions.Variables;
using Chel.Commands;
using Chel.Exceptions;
using Chel.Parsing;
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
                    Substitute.For<ICommandParameterBinder>(),
                    Substitute.For<IScriptProvider>(),
                    _ => { }
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
                    Substitute.For<ICommandParameterBinder>(),
                    Substitute.For<IScriptProvider>(),
                    _ => { }
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
                    null,
                    Substitute.For<IScriptProvider>(),
                    _ => { }
                );

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("parameterBinder", ex.ParamName);
        }

        [Fact]
        public void Ctor_ScriptProvidersIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Session(
                    Substitute.For<IParser>(),
                    Substitute.For<ICommandFactory>(),
                    Substitute.For<ICommandParameterBinder>(),
                    null,
                    _ => { }
                );

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("scriptProvider", ex.ParamName);
        }

        [Fact]
        public void Ctor_ResultHandlerIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Session(
                    Substitute.For<IParser>(),
                    Substitute.For<ICommandFactory>(),
                    Substitute.For<ICommandParameterBinder>(),
                    Substitute.For<IScriptProvider>(),
                    null
                );

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("resultHandler", ex.ParamName);
        }

        [Fact]
        public void Execute_InputIsNull_DoNothing()
        {
            // arrange
            var sut = CreateSession(_ => {});

            // act, assert
            sut.Execute(null);
        }

        [Fact]
        public void Execute_SingleCommandNotRegistered_PassesCommandNotFoundResultToHandler()
        {
            // arrange
            CommandResult executionResult = null;
            var sut = CreateSession(result => executionResult = result);
            
            // act
            sut.Execute("sample");

            // assert
            Assert.IsType<FailureResult>(executionResult);
            var failureResult = executionResult as FailureResult;
            Assert.Equal(new SourceLocation(1, 1), failureResult.SourceLocation);
        }

        [Fact]
        public void Execute_ErrorOnLine2_ReportsFailureOnLine2()
        {
            // arrange
            CommandResult executionResult = null;
            var sut = CreateSession(result => executionResult = result, typeof(SampleCommand));

            // act
            sut.Execute("sample\nboo");

            // assert
            Assert.IsType<FailureResult>(executionResult);
            var failureResult = executionResult as FailureResult;
            Assert.Equal(new SourceLocation(2, 1), failureResult.SourceLocation);
        }

        [Fact]
        public void Execute_CommandIsScript_ExecutesScript()
        {
            // arrange
            var scriptProvider = Substitute.For<IScriptProvider>();
            scriptProvider.GetScriptSource(null, "script").Returns("echo hi");

            CommandResult executionResult = null;
            var sut = CreateSession(
                result => executionResult = result,
                null,
                scriptProvider,
                typeof(Echo));

            // act
            sut.Execute("script");

            // assert
            var valueResult = Assert.IsType<ValueResult>(executionResult);
            Assert.Equal("hi", valueResult.Value.ToString());
        }

        [Fact]
        public void Execute_ScriptNameSameAsCommandName_ExecutesCommand()
        {
            // arrange
            var scriptProvider = Substitute.For<IScriptProvider>();
            scriptProvider.GetScriptSource(null, "echo").Returns("echo hi");

            CommandResult executionResult = null;
            var sut = CreateSession(
                result => executionResult = result,
                null,
                scriptProvider,
                typeof(Echo));

            // act
            sut.Execute("echo");

            // assert
            var valueResult = Assert.IsType<ValueResult>(executionResult);
            Assert.Empty(valueResult.Value.ToString());
        }

        [Fact]
        public void Execute_Script_PassesCommandResultToHandler()
        {
            // arrange
            CommandResult executionResult = null;
            var sut = CreateSession(result => executionResult = result, typeof(SampleCommand));

            // act
            sut.Execute("sample");
            
            // assert
            Assert.IsType<SuccessResult>(executionResult);
        }

        [Fact]
        public void Execute_ParameterProvided_ParameterSetOnCommand()
        {
            // arrange
            ValueResult executionResult = null;
            var sut = CreateSession(result => executionResult = result as ValueResult, typeof(NumberedParameterCommand));

            // act
            sut.Execute("num p1 p2");
            
            // assert
            Assert.Equal("p1 p2", executionResult.Value.ToString());
        }

        [Fact]
        public void Execute_ParameterBindingHasErrors_FailureResultReturned()
        {
            // arrange
            FailureResult executionResult = null;
            var sut = CreateSession(result => executionResult = result as FailureResult, typeof(NumberedParameterCommand));

            // act
            sut.Execute("num p1 p2 p3");
            
            // assert
            Assert.Equal("ERROR (line 1, character 11): Unexpected numbered parameter 'p3'", executionResult.ToString());
        }

        [Fact]
        public void Execute_ParserThrowsException_FailureResultReturned()
        {
            // arrange
            var parser = Substitute.For<IParser>();
            var location = new SourceLocation(1, 1);
            parser.Parse(Arg.Any<string>()).Returns(x => { throw new ParseException(location, "message"); });

            var factory = Substitute.For<ICommandFactory>();
            var binder = Substitute.For<ICommandParameterBinder>();
            var scriptProvider = Substitute.For<IScriptProvider>();

            FailureResult executionResult = null;
            var sut = new Session(parser, factory, binder, scriptProvider, result => executionResult = result as FailureResult);

            // act
            sut.Execute("command");

            // assert
            Assert.Equal(new SourceLocation(1, 1), executionResult.SourceLocation);
        }

        [Fact]
        public void Execute_FactoryThrowsException_FailureResultReturned()
        {
            // arrange
            var commandBuilder = new CommandInput.Builder(new SourceLocation(1, 1), new ExecutionTargetIdentifier(null, "command"));
            var commandInput = commandBuilder.Build();

            var parser = Substitute.For<IParser>();
            parser.Parse(Arg.Any<string>()).Returns(new[]{ commandInput });

            var factory = Substitute.For<ICommandFactory>();
            factory.Create(Arg.Any<CommandInput>()).Returns(x => { throw new Exception(); });

            var binder = Substitute.For<ICommandParameterBinder>();
            var scriptProvider = Substitute.For<IScriptProvider>();

            FailureResult executionResult = null;
            var sut = new Session(parser, factory, binder, scriptProvider, result => executionResult = result as FailureResult);

            // act
            sut.Execute("command");

            // assert
            Assert.Equal(new SourceLocation(1, 1), executionResult.SourceLocation);
        }

        [Fact]
        public void Execute_CommandThrowsException_FailureResultReturned()
        {
            // arrange
            FailureResult executionResult = null;
            var sut = CreateSession(result => executionResult = result as FailureResult, typeof(ExceptionCommand));

            // act
            sut.Execute("ex");

            // assert
            Assert.Equal(new SourceLocation(1, 1), executionResult.SourceLocation);
        }

        [Fact]
        public void Execute_ParameterIsUnsetVariable_ReturnsFailure()
        {
            // arrange
            FailureResult executionResult = null;
            var sut = CreateSession(result => executionResult = result as FailureResult, typeof(NamedParameterCommand));

            // act
            sut.Execute("nam -param1 $foo$");

            // assert
            Assert.Equal("Variable $foo$ is not set", executionResult.Message);
        }

        [Fact]
        public void Execute_ParameterIsSetVariable_PerformsSubstitution()
        {
            // arrange
            CommandResult executionResult = null;
            var sut = CreateSession(
                result => executionResult = result,
                sessionObjectsConfigurator: x => {
                    x.Register<VariableCollection>();
                    ((VariableCollection)x.Resolve(typeof(VariableCollection))).Set(new Variable("foo", new Literal("lorem")));
                },
                null,
                typeof(NamedParameterCommand));

            // act
            sut.Execute("nam -param1 $foo$ -param2 $foo$");

            // assert
            Assert.IsType<ValueResult>(executionResult);
            Assert.Equal("lorem lorem", (executionResult as ValueResult).Value.ToString());
        }

        [Fact]
        public void Execute_CommandReturnsFailureWithCurrentSourceLineToken_ReportsProperSourceLine()
        {
            // arrange
            FailureResult executionResult = null;
            var sut = CreateSession(result => executionResult = (FailureResult)result, typeof(FailureCommand));

            // act
            sut.Execute("fail");

            // assert
            Assert.Equal(new SourceLocation(1, 1), executionResult.SourceLocation);
        }

        [Fact]
        public void Execute_CommandIncludesSubcommand_ExpandsSubcommandBeforeExecution()
        {
            // arrange
            ValueResult executionResult = null;
            var sut = CreateSession(result => executionResult = (ValueResult)result, typeof(NumberedParameterCommand));

            // act
            sut.Execute("num << (num 1 2) 3");
            
            // assert
            Assert.Equal("1 2 3", executionResult.Value.ToString());
        }

        [Fact]
        public void Execute_CommandIncludesManySubcommands_ExpandsSubcommandsBeforeExecution()
        {
            // arrange
            ValueResult executionResult = null;
            var sut = CreateSession(result => executionResult = (ValueResult)result, typeof(NumberedParameterCommand));

            // act
            sut.Execute("num << (num 1 2) <<(num 3 4)");
            
            // assert
            Assert.Equal("1 2 3 4", executionResult.Value.ToString());
        }

        [Fact]
        public void Execute_CommandIncludesSubSubcommand_ExpandsSubcommandsBeforeExecution()
        {
            // arrange
            ValueResult executionResult = null;
            var sut = CreateSession(result => executionResult = (ValueResult)result, typeof(NumberedParameterCommand));

            // act
            sut.Execute("num << (num 1 << (num 2 3)) 4");
            
            // assert
            Assert.Equal("1 2 3 4", executionResult.Value.ToString());
        }

        [Fact]
        public void Execute_CommandIncludesSubcommandInList_ExpandsSubcommandBeforeExecution()
        {
            // arrange
            ValueResult executionResult = null;
            var sut = CreateSession(result => executionResult = (ValueResult)result, typeof(ListParameterCommand), typeof(NumberedParameterCommand));

            // act
            sut.Execute("list-params -array [1 << (num 2 3) 4]");
            
            // assert
            Assert.Equal("[ 1 (2 3) 4 ]", executionResult.Value.ToString());
        }

        [Fact]
        public void Execute_CommandIncludesManySubcommandsInList_ExpandsSubcommandsBeforeExecution()
        {
            // arrange
            ValueResult executionResult = null;
            var sut = CreateSession(result => executionResult = (ValueResult)result, typeof(ListParameterCommand), typeof(NumberedParameterCommand));

            // act
            sut.Execute("list-params -array [<< (num 1 2) << (num 3 4)]");
            
            // assert
            Assert.Equal("[ (1 2) (3 4) ]", executionResult.Value.ToString());
        }

        [Fact]
        public void Execute_CommandIncludesSubSubcommandInList_ExpandsSubcommandsBeforeExecution()
        {
            // arrange
            ValueResult executionResult = null;
            var sut = CreateSession(result => executionResult = (ValueResult)result, typeof(ListParameterCommand), typeof(NumberedParameterCommand));

            // act
            sut.Execute("list-params -array [ << (num 1 << (num 2 3)) 4]");
            
            // assert
            Assert.Equal("[ (1 2 3) 4 ]", executionResult.Value.ToString());
        }

        // todo: tests for subcommands in maps

        [Fact]
        public void Execute_SubcommandFails_FailureResultReturned()
        {
            // arrange
            FailureResult executionResult = null;
            var sut = CreateSession(result => executionResult = (FailureResult)result, typeof(NumberedParameterCommand), typeof(FailureCommand));

            // act
            sut.Execute("num << fail 2");
            
            // assert
            Assert.Equal(new SourceLocation(1, 8), executionResult.SourceLocation);
        }

        [Fact]
        public void Execute_SubSubcommandFails_FailureResultReturned()
        {
            // arrange
            FailureResult executionResult = null;
            var sut = CreateSession(result => executionResult = (FailureResult)result, typeof(NumberedParameterCommand), typeof(FailureCommand));

            // act
            sut.Execute("num << (num << fail 2) 3");
            
            // assert
            Assert.Equal(new SourceLocation(1, 16), executionResult.SourceLocation);
        }

        [Fact]
        public void Execute_UnknownSubcommand_FailureResultReturned()
        {
            // arrange
            FailureResult executionResult = null;
            var sut = CreateSession(result => executionResult = (FailureResult)result, typeof(NumberedParameterCommand));

            // act
            sut.Execute("num << unknown");
            
            // assert
            Assert.Equal(new SourceLocation(1, 8), executionResult.SourceLocation);
            Assert.Equal("Unknown command 'unknown'", executionResult.Message);
        }

        [Fact]
        public void Execute_UnknownSubcommand_ReportsErrorOnLine2()
        {
            // arrange
            FailureResult executionResult = null;
            var sut = CreateSession(result => executionResult = (FailureResult)result, typeof(NumberedParameterCommand));

            // act
            sut.Execute("num << (\nunknown)");
            
            // assert
            Assert.Equal(new SourceLocation(2, 1), executionResult.SourceLocation);
        }

        private Session CreateSession(Action<CommandResult> resultHandler, params Type[] commandTypes)
        {
            return CreateSession(resultHandler, null, null, commandTypes);
        }

        private Session CreateSession(
            Action<CommandResult> resultHandler,
            Action<ScopedObjectRegistry> sessionObjectsConfigurator = null,
            IScriptProvider scriptProvider = null,
            params Type[] commandTypes)
        {
            var nameValidator = new NameValidator();
            var parser = new Parser(nameValidator);
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

            if(scriptProvider == null)
            {
                scriptProvider = Substitute.For<IScriptProvider>();
                scriptProvider.GetScriptSource(Arg.Any<string>(), Arg.Any<string>()).Returns((string)null);
            }

            return new Session(parser, factory, binder, scriptProvider, resultHandler);
        }
    }
}