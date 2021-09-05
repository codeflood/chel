using System;
using System.Linq;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Abstractions.Variables;
using NSubstitute;
using Xunit;

namespace Chel.UnitTests.Commands
{
    public class VarTests
    {
        [Fact]
        public void Ctor_VariablesIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Var(null, Substitute.For<IPhraseDictionary>(), Substitute.For<INameValidator>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variables", ex.ParamName);
        }

        [Fact]
        public void Ctor_PhraseDictionaryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Var(new VariableCollection(), null, Substitute.For<INameValidator>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("phraseDictionary", ex.ParamName);
        }

                [Fact]
        public void Ctor_NameValidatorIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Var(new VariableCollection(), Substitute.For<IPhraseDictionary>(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("nameValidator", ex.ParamName);
        }

        [Fact]
        public void Execute_NoParametersNoVariables_ReturnsEmptyMessage()
        {
            // arrange
            var sut = CreateVarCommand();

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Equal("No variables set.", result.Value.ToString());
        }

        [Fact]
        public void Execute_NoParametersVariablesSet_ReturnsVariablesNamesAndValues()
        {
            // arrange
            var sut = CreateVarCommand(variables => {
                variables.Set(new Variable("name1", new Literal("value1")));
                variables.Set(new Variable("name2", new Literal("value2")));
            });

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Matches(@"name1\s+value1", result.Value.ToString());
            Assert.Matches(@"name2\s+value2", result.Value.ToString());
        }

        [Fact]
        public void Execute_NameProvidedNoVariables_ReturnsNotSet()
        {
            // arrange
            var sut = CreateVarCommand();
            sut.Name = "name";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Equal("Variable 'name' is not set.", result.Value.ToString());
        }

        [Fact]
        public void Execute_NameProvidedVariableSet_ReturnsVariableValue()
        {
            // arrange
            var sut = CreateVarCommand(variables => {
                variables.Set(new Variable("name", new Literal("value")));
            });
            sut.Name = "name";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Equal("value", result.Value.ToString());
        }

        [Fact]
        public void Execute_InvalidNameProvided_ReturnsError()
        {
            // arrange
            var sut = CreateVarCommand(variables => {
                variables.Set(new Variable("name", new Literal("value")));
            });
            sut.Name = "invalid:name";

            // act
            var result = sut.Execute();

            // assert
            var failureResult = Assert.IsType<FailureResult>(result);
            Assert.Contains("Invalid character in variable name 'invalid:name'.", failureResult.Messages);
        }

        [Fact]
        public void Execute_NameProvidedListVariableSet_ReturnsListVariableValue()
        {
            // arrange
            var sut = CreateVarCommand(variables => {
                variables.Set(new Variable("name", new List(new[] {
                    new Literal("val1"),
                    new Literal("val2")
                })));
            });
            sut.Name = "name";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Equal("[ val1 val2 ]", result.Value.ToString());
        }

        [Fact]
        public void Execute_ValueProvided_SetsVariable()
        {
            // arrange
            VariableCollection variables = null;
            var sut = CreateVarCommand(x => variables = x);
            sut.Name = "name";
            sut.Value = new Literal("value");

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Equal("value", result.Value.ToString());

            var variable = variables.Get("name");
            Assert.Equal("value", variable.Value.ToString());
        }

        [Fact]
        public void Execute_ListProvided_SetsVariableAsList()
        {
            // arrange
            VariableCollection variables = null;
            var sut = CreateVarCommand(x => variables = x);
            sut.Name = "name";
            sut.Value = new List(new[] {
                new Literal("val1"),
                new Literal("val2")
            });

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            var variable = variables.Get("name");
            var values = ((List)variable.Value).Values.Select(x => x.ToString());
            Assert.Equal(new[]{ "val1", "val2" }, values);
        }

        private Var CreateVarCommand(Action<VariableCollection> variableConfigurator = null)
        {
            var variables = new VariableCollection();

            if(variableConfigurator != null)
                variableConfigurator.Invoke(variables);

            return new Var(variables, new PhraseDictionary(), new NameValidator());
        }
    }
}