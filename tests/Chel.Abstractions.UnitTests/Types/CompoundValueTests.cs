using System;
using System.Collections.Generic;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests.Types
{
    public class CompoundValueTests
    {
        [Fact]
        public void Ctor_NullLiteral_SetsPropertyToEmptyCollection()
        {
            // arrange, act
            var sut = new CompoundValue((Literal)null);

            // assert
            Assert.Empty(sut.Values);
        }

        [Fact]
        public void Ctor_LiteralProvided_SetsProperty()
        {
            // arrange
            var value = new Literal("lit");

            // act
            var sut = new CompoundValue(value);

            // assert
            Assert.Equal(new[] { value }, sut.Values);
        }

        [Fact]
        public void Ctor_NullVariableReference_SetsPropertyToEmptyCollection()
        {
            // arrange, act
            var sut = new CompoundValue((VariableReference)null);

            // assert
            Assert.Empty(sut.Values);
        }

        [Fact]
        public void Ctor_VariableReferenceProvided_SetsProperty()
        {
            // arrange
            var value = new VariableReference("var");

            // act
            var sut = new CompoundValue(value);

            // assert
            Assert.Equal(new[] { value }, sut.Values);
        }

        [Theory]
        [MemberData(nameof(Ctor_ListContainsUnexpectedTypes_ThrowsException_DataSource))]
        public void Ctor_ListContainsUnexpectedTypes_ThrowsException(IReadOnlyList<ChelType> input)
        {
            // arrange
            Action sutAction = () => new CompoundValue(input);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("values", ex.ParamName);
        }

        public static IEnumerable<object[]> Ctor_ListContainsUnexpectedTypes_ThrowsException_DataSource()
        {
            yield return new[] {
                new ChelType[] {
                    new Literal("val"),
                    new List(new[] { new Literal("val") })
                }
            };

            yield return new[] {
                new ChelType[] {
                    new CompoundValue(new Literal("val"))
                }
            };
        }
    }
}