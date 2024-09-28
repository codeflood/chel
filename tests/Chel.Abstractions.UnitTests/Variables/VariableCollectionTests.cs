using System;
using Chel.Abstractions.Types;
using Chel.Abstractions.Variables;
using Xunit;

namespace Chel.Abstractions.UnitTests.Variables
{
    public class VariableCollectionTests
    {
        [Fact]
        public void Set_VariableIsNull_ThrowsException()
        {
            // arrange
            var sut = new VariableCollection();
            Action sutAction = () => sut.Set(null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variable", ex.ParamName);
        }

        [Fact]
        public void Set_WhenCalled_StoresVariable()
        {
            // arrange
            var sut = new VariableCollection();
            var value = new Literal("alpha");
            var variable = new Variable("name", value);

            // act
            sut.Set(variable);

            // assert
            var result = sut.Get("name");
            Assert.Equal(value, result!.Value);
        }

        [Fact]
        public void SetGet_VariableAlreadySet_ReplacesVariable()
        {
            // arrange
            var sut = new VariableCollection();
            var variable1 = new Variable("name", new Literal("alpha"));
            var variable2 = new Variable("name", new Literal("beta"));
            sut.Set(variable1);

            // act
            sut.Set(variable2);

            // assert
            var result = sut.Get("name");
            Assert.Equal("beta", result!.Value.ToString());
        }

        [Fact]
        public void Get_VariableNotSet_ReturnsNull()
        {
            // arrange
            var sut = new VariableCollection();

            // act
            var result = sut.Get("name");

            // assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("name")]
        [InlineData("NAME")]
        [InlineData("NaMe")]
        public void Get_DifferentCasingForName_ReturnsVariable(string name)
        {
            // arrange
            var sut = new VariableCollection();
            var value = new Literal("alpha");
            var variable = new Variable("name", value);
            sut.Set(variable);

            // act
            var result = sut.Get(name);

            // assert
            Assert.Equal(variable, result);
        }

        [Fact]
        public void Remove_NameIsNull_ThrowsException()
        {
            // arrange
            var sut = new VariableCollection();
            sut.Set(new Variable("name", new Literal("value")));
            
            Action sutAction = () => sut.Remove(null!);

            // act
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void Remove_NameIsEmpty_ThrowsException()
        {
            // arrange
            var sut = new VariableCollection();
            sut.Set(new Variable("name", new Literal("value")));
            
            Action sutAction = () => sut.Remove("");

            // act
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void Remove_NoVariablesSet_DoesNothing()
        {
            // arrange
            var sut = new VariableCollection();

            // act, assert
            sut.Remove("name");
        }

        [Theory]
        [InlineData("name")]
        [InlineData("NAME")]
        [InlineData("NaMe")]
        public void Remove_VariableWasSet_RemovesVariable(string name)
        {
            // arrange
            var sut = new VariableCollection();
            var variable = new Variable("name", new Literal("alpha"));
            sut.Set(variable);

            // act
            sut.Remove(name);

            // assert
            var result = sut.Get("name");
            Assert.Null(result);
        }

        [Fact]
        public void Remove_VariableNotSet_DoesNothing()
        {
            // arrange
            var sut = new VariableCollection();
            var variable = new Variable("name", new Literal("alpha"));
            sut.Set(variable);

            // act
            sut.Remove("othername");

            // assert
            var names = sut.Names;
            Assert.Equal(new[]{ "name" }, names);
        }

        [Fact]
        public void Names_NothingSet_ReturnsEmpty()
        {
            // arrange
            var sut = new VariableCollection();

            // act
            var names = sut.Names;

            // assert
            Assert.Empty(names);
        }

        [Fact]
        public void Names_VariablesSet_ReturnsNames()
        {
            // arrange
            var sut = new VariableCollection();
            sut.Set(new Variable("name1", new Literal("value1")));
            sut.Set(new Variable("name2", new Literal("value2")));

            // act
            var names = sut.Names;

            // assert
            Assert.Equal(new[] { "name1", "name2" }, names);
        }
    }
}