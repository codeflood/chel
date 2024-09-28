using System;
using System.Linq;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Abstractions.Variables;
using Chel.Commands;
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
            Action sutAction = () => new Var(null!, Substitute.For<INameValidator>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variables", ex.ParamName);
        }

        [Fact]
        public void Ctor_NameValidatorIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Var(new VariableCollection(), null!);

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
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Equal("No variables set.", result.Value.ToString());
        }

        [Fact]
        public void Execute_NoParametersVariablesSet_ReturnsVariablesAsMap()
        {
            // arrange
            var sut = CreateVarCommand(variables => {
                variables.Set(new Variable("name1", new Literal("value1")));
                variables.Set(new Variable("name2", new Literal("value2")));
            });

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            var vars = Assert.IsType<Map>(result.Value);
            Assert.Equal(2, vars.Entries.Count);
            Assert.Equal("value1", ((Literal)vars.Entries["name1"]).Value);
            Assert.Equal("value2", ((Literal)vars.Entries["name2"]).Value);
        }

        [Fact]
        public void Execute_NameProvidedNoVariables_ReturnsNotSet()
        {
            // arrange
            var sut = CreateVarCommand();
            sut.Name = "name";

            // act
            var result = (ValueResult)sut.Execute();

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
            var result = (ValueResult)sut.Execute();

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
            Assert.Equal("Invalid character in variable name 'invalid:name'.", failureResult.Message);
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
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Equal("[ val1 val2 ]", result.Value.ToString());
        }

        [Fact]
        public void Execute_ValueProvided_SetsVariable()
        {
            // arrange
            VariableCollection variables = null!;
            var sut = CreateVarCommand(x => variables = x);
            sut.Name = "name";
            sut.Value = new Literal("value");

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Equal("value", result.Value.ToString());

            var variable = variables.Get("name");
            Assert.Equal("value", variable!.Value.ToString());
        }

        [Fact]
        public void Execute_ListProvided_SetsVariableAsList()
        {
            // arrange
            VariableCollection variables = null!;
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
            var values = ((List)variable!.Value).Values.Select(x => x.ToString());
            Assert.Equal(new[]{ "val1", "val2" }, values);
        }

        [Fact]
        public void Execute_ClearVariableNameNotProvided_ReturnsError()
        {
            // arrange
            var sut = CreateVarCommand();
            sut.Clear = true;

            // act
            var result = sut.Execute();

            // assert
            var failureResult = Assert.IsType<FailureResult>(result);
            Assert.Equal("Missing variable name.", failureResult.Message);
        }

        [Fact]
        public void Execute_ClearVariableNotSet_ReturnsNotSetMessage()
        {
            // arrange
            var sut = CreateVarCommand();
            sut.Name = "name";
            sut.Clear = true;

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Equal("Variable 'name' is not set.", result.Value.ToString());
        }

        [Fact]
        public void Execute_ClearVariableIsSet_RemovesVariable()
        {
            // arrange
            VariableCollection variables = null!;
            var sut = CreateVarCommand(x => {
                variables = x;
                x.Set(new Variable("name", new Literal("value")));
            });
            sut.Name = "name";
            sut.Clear = true;

            // act
            var result = (ValueResult)sut.Execute();

            // assert
            Assert.Equal("Variable 'name' has been cleared.", result.Value.ToString());
            var found = variables.Get("name");
            Assert.Null(found);
        }

        private Var CreateVarCommand(Action<VariableCollection>? variableConfigurator = null)
        {
            var variables = new VariableCollection();

            if(variableConfigurator != null)
                variableConfigurator.Invoke(variables);

            return new Var(variables, new NameValidator());
        }
    }
}