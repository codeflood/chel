using System;
using System.Collections.Generic;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests.Types
{
    public class SingleValueTests
    {
        [Fact]
        public void Ctor_NullLiteral_SetsPropertyToEmptyCollection()
        {
            // arrange, act
            var sut = new SingleValue((Literal)null);

            // assert
            Assert.Empty(sut.Values);
        }

        [Fact]
        public void Ctor_LiteralProvided_SetsProperty()
        {
            // arrange
            var value = new Literal("lit");

            // act
            var sut = new SingleValue(value);

            // assert
            Assert.Equal(new[] { value }, sut.Values);
        }

        [Fact]
        public void Ctor_NullVariableReference_SetsPropertyToEmptyCollection()
        {
            // arrange, act
            var sut = new SingleValue((VariableReference)null);

            // assert
            Assert.Empty(sut.Values);
        }

        [Fact]
        public void Ctor_VariableReferenceProvided_SetsProperty()
        {
            // arrange
            var value = new VariableReference("var");

            // act
            var sut = new SingleValue(value);

            // assert
            Assert.Equal(new[] { value }, sut.Values);
        }

        [Fact]
        public void Ctor_NullList_SetsPropertyToEmptyCollection()
        {
            // arrange, act
            var sut = new SingleValue((IReadOnlyList<ChelType>)null);

            // assert
            Assert.Empty(sut.Values);
        }

        [Fact]
        public void Ctor_ListProvided_SetsProperty()
        {
            // arrange
            var val1 = new Literal("val1");
            var val2 = new Literal("val2");

            // act
            var sut = new SingleValue(new List<ChelType> { val1, val2 });

            // assert
            Assert.Equal(new[] { val1, val2 }, sut.Values);
        }
    }
}