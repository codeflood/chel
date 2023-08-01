using System;
using System.Collections.Generic;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Commands.Conditions;
using Xunit;

namespace Chel.UnitTests.Commands.Conditions
{
    public class EqualsTests
    {
        [Fact]
        public void Ctor_PhraseDictionaryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Equals(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("phraseDictionary", ex.ParamName);
        }

        [Fact]
        public void Execute_FirstOperandIsNull_ReturnsFailure()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.FirstOperand = null;

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public void Execute_SecondOperandIsNull_ReturnsFailure()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.FirstOperand = new Literal("a");
            sut.SecondOperand = null;

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Theory]
        [MemberData(nameof(Execute_OperandsEqual_DataSource))]
        public void Execute_OperandsAreEqual_ReturnsTrue(ChelType operand1, ChelType operand2)
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.FirstOperand = operand1;
            sut.SecondOperand = operand2;

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal("true", literalValue.Value);
        }

        [Theory]
        [MemberData(nameof(Execute_OperandsNotEqual_DataSource))]
        public void Execute_StringOperandsNotEqual_ReturnsTrue(ChelType operand1, ChelType operand2)
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.FirstOperand = operand1;
            sut.SecondOperand = operand2;

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal("false", literalValue.Value);
        }

        public static IEnumerable<object[]> Execute_OperandsEqual_DataSource()
        {
            yield return new[]
            {
                new Literal("a"),
                new Literal("a")
            };

            yield return new[]
            {
                new List(new[]
                {
                    new Literal("a"),
                    new Literal("b")
                }),
                new List(new[]
                {
                    new Literal("a"),
                    new Literal("b"
                )})
            };
        }

        public static IEnumerable<object[]> Execute_OperandsNotEqual_DataSource()
        {
            yield return new[]
            {
                new Literal("a"),
                new Literal("b")
            };

            yield return new[]
            {
                new List(new[]
                {
                    new Literal("a"),
                    new Literal("b")
                }),
                new List(new[]
                {
                    new Literal("b"),
                    new Literal("a"
                )})
            };

            yield return new[]
            {
                new List(new[]
                {
                    new Literal("a"),
                    new Literal("b"),
                    new Literal("c")
                }),
                new List(new[]
                {
                    new Literal("a"),
                    new Literal("b"
                )})
            };
        }

        [Fact]
        public void Execute_NumericOperand1IsNotNumeric_ReturnsError()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsNumeric = true;
            sut.FirstOperand = new Literal("abc");
            sut.SecondOperand = new Literal("42");

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public void Execute_NumericOperand2IsNotNumeric_ReturnsError()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsNumeric = true;
            sut.FirstOperand = new Literal("10.00");
            sut.SecondOperand = new Literal("abc");

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public void Execute_NumericOperandsAreEqual_ReturnsTrue()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsNumeric = true;
            sut.FirstOperand = new Literal("10.00");
            sut.SecondOperand = new Literal("10");

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal("true", literalValue.Value);
        }

        [Fact]
        public void Execute_NumericOperandsNotEqual_ReturnsTrue()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsNumeric = true;
            sut.FirstOperand = new Literal("7");
            sut.SecondOperand = new Literal("9");

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal("false", literalValue.Value);
        }

        // dates
        // guids

        private Equals CreateEqualsCommand()
        {
            return new Equals(new PhraseDictionary());
        }
    }
}