using System;
using System.Collections.Generic;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests.Types
{
    public class VariableReferenceTests
    {
        [Fact]
        public void Ctor_VariableNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new VariableReference(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("variableName", ex.ParamName);
        }

        [Fact]
        public void Ctor_VariableNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new VariableReference("");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("variableName", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new VariableReference("var");

            // assert
            Assert.Equal("var", sut.VariableName);
            Assert.Empty(sut.SubReferences);
        }

        [Fact]
        public void Ctor_SubReferencesProvided_SetsProperties()
        {
            // arrange, act
            var subreferences = new[] { "1", "2" };
            var sut = new VariableReference("var", subreferences);

            // assert
            Assert.Equal("var", sut.VariableName);
            Assert.Equal(subreferences, sut.SubReferences);
        }

        [Theory]
        [MemberData(nameof(EqualVariableReferenceDataSource))]
        public void Equals_ValuesAreEqual_ReturnsTrue(VariableReference sut1, VariableReference sut2)
        {
            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(EqualVariableReferenceDataSource))]
        public void GetHashCode_ValuesAreEqual_ReturnsSameHashCode(VariableReference sut1, VariableReference sut2)
        {
            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashCode1, hashCode2);
        }

        public static IEnumerable<object[]> EqualVariableReferenceDataSource()
        {
            yield return new[] {
                new VariableReference("var1"),
                new VariableReference("var1")
            };

            yield return new[] {
                new VariableReference("var1", new[] { "a" }),
                new VariableReference("var1", new[] { "a" })
            };
        }

        [Theory]
        [MemberData(nameof(NotEqualVariableReferenceDataSource))]
        public void Equals_ValuesAreDifferent_ReturnsFalse(VariableReference sut1, VariableReference sut2)
        {
            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(NotEqualVariableReferenceDataSource))]
        public void GetHashCode_ValuesAreNotEqual_ReturnsDifferentHashCodes(VariableReference sut1, VariableReference sut2)
        {
            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // act, assert
            Assert.NotEqual(hashCode1, hashCode2);
        }

        public static IEnumerable<object[]> NotEqualVariableReferenceDataSource()
        {
            yield return new[] {
                new VariableReference("var1"),
                new VariableReference("var2")
            };

            yield return new[] {
                new VariableReference("var1", new[] { "1" }),
                new VariableReference("var1", new[] { "2" })
            };
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsVariableName()
        {
            // arrange
            var sut = new VariableReference("var");

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("$var$", result);
        }

        [Fact]
        public void ToString_SubReferencesSet_ReturnsSubReferences()
        {
            // arrange
            var sut = new VariableReference("var", new[] { "1", "2" });

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("$var:1:2$", result);
        }
    }
}
