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
            Action sutAction = () => new Var(null, Substitute.For<IPhraseDictionary>());

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variables", ex.ParamName);
        }

        [Fact]
        public void Ctor_PhraseDictionaryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Var(new VariableCollection(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("phraseDictionary", ex.ParamName);
        }

        [Fact]
        public void Execute_NoParametersNoVariables_ReturnsEmptyMessage()
        {
            // arrange
            var variables = new VariableCollection();
            var sut = new Var(variables, new PhraseDictionary());

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Equal("No variables set.", result.Value);
        }

        [Fact]
        public void Execute_NoParametersVariablesSet_ReturnsVariablesNamesAndValues()
        {
            // arrange
            var variables = new VariableCollection();
            variables.Set(new Variable("name1", new Literal("value1")));
            variables.Set(new Variable("name2", new Literal("value2")));

            var sut = new Var(variables, new PhraseDictionary());

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Matches(@"name1\s+value1", result.Value);
            Assert.Matches(@"name2\s+value2", result.Value);
        }

        [Fact]
        public void Execute_NameProvidedNoVariables_ReturnsNotSet()
        {
            // arrange
            var sut = new Var(new VariableCollection(), new PhraseDictionary());
            sut.Name = "name";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Equal("Variable 'name' is not set.", result.Value);
        }

        [Fact]
        public void Execute_NameProvidedVariableSet_ReturnsVariableValue()
        {
            // arrange
            var variables = new VariableCollection();
            variables.Set(new Variable("name", new Literal("value")));

            var sut = new Var(variables, new PhraseDictionary());
            sut.Name = "name";

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Equal("value", result.Value);
        }

        [Fact]
        public void Execute_ValueProvided_SetsVariable()
        {
            // arrange
            var variables = new VariableCollection();

            var sut = new Var(variables, new PhraseDictionary());
            sut.Name = "name";
            sut.Value = new Literal("value");

            // act
            var result = sut.Execute() as ValueResult;

            // assert
            Assert.Equal("value", result.Value);

            var variable = variables.Get("name");
            Assert.Equal("value", variable.Value.ToString());
        }

        [Fact]
        public void Execute_ListProvided_SetsVariableAsList()
        {
            // arrange
            var variables = new VariableCollection();

            var sut = new Var(variables, new PhraseDictionary());
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
    }
}