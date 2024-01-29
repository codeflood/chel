using System;
using System.Collections.Generic;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Commands.Conditions;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Commands.Conditions
{
    public class EqualsTests
    {
        [Fact]
        public void Ctor_ParameterParserIsNull_Throws()
        {
            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(() => new Equals(null));
            Assert.Equal("parameterParser", ex.ParamName);
        }

        [Fact]
        public void Execute_FirstOperandIsNull_ReturnsFailure()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.FirstOperand = null;
            sut.SecondOperand = new Literal("a");

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
            Assert.Equal(Constants.TrueLiteral, literalValue.Value);
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
            Assert.Equal(Constants.FalseLiteral, literalValue.Value);
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
            Assert.Equal(Constants.TrueLiteral, literalValue.Value);
        }

        [Fact]
        public void Execute_NumericOperandsNotEqual_ReturnsFalse()
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
            Assert.Equal(Constants.FalseLiteral, literalValue.Value);
        }

        [Theory]
        [MemberData(nameof(Execute_MultipleTreatmentsSet_ReturnsFalse_DataSource))]
        public void Execute_MultipleTreatmentsSet_ReturnsFailure(Equals sut)
        {
            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        public static IEnumerable<object[]> Execute_MultipleTreatmentsSet_ReturnsFalse_DataSource()
        {
            var numericAndDateEquals = CreateEqualsCommand();
            numericAndDateEquals.FirstOperand = new Literal("1");
            numericAndDateEquals.SecondOperand = new Literal("2");
            numericAndDateEquals.IsNumeric = true;
            numericAndDateEquals.IsDate = true;

            yield return new[] { 
                numericAndDateEquals
            };

            var numericAndGuidEquals = CreateEqualsCommand();
            numericAndGuidEquals.FirstOperand = new Literal("1");
            numericAndGuidEquals.SecondOperand = new Literal("2");
            numericAndGuidEquals.IsNumeric = true;
            numericAndGuidEquals.IsGuid = true;
            
            yield return new[] { 
                numericAndGuidEquals
            };

            var dateAndGuidEquals = CreateEqualsCommand();
            dateAndGuidEquals.FirstOperand = new Literal("2023-05-04");
            dateAndGuidEquals.SecondOperand = new Literal("2023-05-04");
            dateAndGuidEquals.IsDate = true;
            dateAndGuidEquals.IsGuid = true;

            yield return new[] { 
                dateAndGuidEquals
            };
        }

        [Fact]
        public void Execute_DateOperand1IsNotDate_ReturnsError()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsDate = true;
            sut.FirstOperand = new Literal("abc");
            sut.SecondOperand = new Literal("2023-08-08 12:13:14");

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public void Execute_DateOperand2IsNotDate_ReturnsError()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsDate = true;
            sut.FirstOperand = new Literal("2023-08-08 12:13:14");
            sut.SecondOperand = new Literal("abc");

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public void Execute_DateOperandsAreEqual_ReturnsTrue()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsDate = true;
            sut.FirstOperand = new Literal("2023-08-08 12:13:14");
            sut.SecondOperand = new Literal("2023-08-08T12:13:14");

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal(Constants.TrueLiteral, literalValue.Value);
        }

        [Fact]
        public void Execute_DateOperandsNotEqual_ReturnsFalse()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsDate = true;
            sut.FirstOperand = new Literal("2023-08-08 12:13:14");
            sut.SecondOperand = new Literal("2023-08-08 12:13:15");

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal(Constants.FalseLiteral, literalValue.Value);
        }

        [Fact]
        public void Execute_GuidOperand1IsNotGuid_ReturnsError()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsGuid = true;
            sut.FirstOperand = new Literal("abc");
            sut.SecondOperand = new Literal("61a76fd0-ac65-47a9-95d5-24bd7cb5f149");

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public void Execute_GuidOperand2IsNotGuid_ReturnsError()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsGuid = true;
            sut.FirstOperand = new Literal("61a76fd0-ac65-47a9-95d5-24bd7cb5f149");
            sut.SecondOperand = new Literal("abc");

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public void Execute_GuidOperandsAreEqual_ReturnsTrue()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsGuid = true;
            sut.FirstOperand = new Literal("61a76fd0-ac65-47a9-95d5-24bd7cb5f149");
            sut.SecondOperand = new Literal("{61A76FD0-AC65-47A9-95D5-24BD7CB5F149}");

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal(Constants.TrueLiteral, literalValue.Value);
        }

        [Fact]
        public void Execute_GuidOperandsNotEqual_ReturnsFalse()
        {
            // arrange
            var sut = CreateEqualsCommand();
            sut.IsGuid = true;
            sut.FirstOperand = new Literal("61a76fd0-ac65-47a9-95d5-24bd7cb5f149");
            sut.SecondOperand = new Literal("71a76fd0-ac65-47a9-95d5-24bd7cb5f149");

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal(Constants.FalseLiteral, literalValue.Value);
        }

        private static Equals CreateEqualsCommand()
        {
            return new Equals(new ParameterParser());
        }
    }
}
