using System;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class VariableCommandParameterTests
    {
        [Fact]
        public void Ctor_VariableNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new VariableCommandParameter(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variableName", ex.ParamName);
        }

        [Fact]
        public void Ctor_VariableNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new VariableCommandParameter("");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("variableName", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new VariableCommandParameter("var");

            // assert
            Assert.Equal("var", sut.VariableName);
        }

        [Fact]
        public void Equals_ValuesAreEqual_ReturnsTrue()
        {
            // arrange
            var sut1 = new VariableCommandParameter("var1");
            var sut2 = new VariableCommandParameter("var1");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ValuesAreDifferent_ReturnsFalse()
        {
            // arrange
            var sut1 = new VariableCommandParameter("var1");
            var sut2 = new VariableCommandParameter("var2");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ValuesAreEqual_ReturnsSameHashCode()
        {
            // arrange
            var sut1 = new VariableCommandParameter("var1");
            var sut2 = new VariableCommandParameter("var1");

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_ValuesAreNotEqual_ReturnsDifferentHashCodes()
        {
            // arrange
            var sut1 = new VariableCommandParameter("var1");
            var sut2 = new VariableCommandParameter("var2");

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // act, assert
            Assert.NotEqual(hashCode1, hashCode2);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsVariableName()
        {
            // arrange
            var sut = new VariableCommandParameter("var");

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("$var$", result);
        }
    }
}