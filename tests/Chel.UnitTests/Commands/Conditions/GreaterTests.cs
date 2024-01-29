using System;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Commands.Conditions;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Commands.Conditions
{
    public class GreaterTests
    {
        [Fact]
        public void Ctor_ParameterParserIsNull_Throws()
        {
            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(() => new Greater(null));
            Assert.Equal("parameterParser", ex.ParamName);
        }

        [Fact]
        public void Execute_FirstOperandIsNull_ReturnsFailure()
        {
            // arrange
            var sut = CreateGreaterCommand();
            sut.Base = null;
            sut.Value = new Literal("1");

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public void Execute_SecondOperandIsNull_ReturnsFailure()
        {
            // arrange
            var sut = CreateGreaterCommand();
            sut.Base = new Literal("1");
            sut.Value = null;

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public void Execute_BaseIsString_ReturnsFailure()
        {
            // arrange
            var sut = CreateGreaterCommand();
            sut.Base = new Literal("a");
            sut.Value = new Literal("1");

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public void Execute_ValueIsString_ReturnsFailure()
        {
            // arrange
            var sut = CreateGreaterCommand();
            sut.Base = new Literal("1");
            sut.Value = new Literal("a");

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<FailureResult>(result);
        }

        [Theory]
        [InlineData("1", "2")]
        [InlineData("1.1", "1.1001")]
        [InlineData("-3.2", "-3.1")]
        [InlineData("-3.2", "5")]
        public void Execute_ValueGreaterThanBase_ReturnsTrueLiteral(string baseValue, string value)
        {
            // arrange
            var sut = CreateGreaterCommand();
            sut.Base = new Literal(baseValue);
            sut.Value = new Literal(value);

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal(Constants.TrueLiteral, literalValue.Value);
        }

        [Theory]
        [InlineData("2", "1")]
        [InlineData("1.1001", "1.1")]
        [InlineData("-3.1", "-3.2")]
        [InlineData("5", "-3.2")]
        public void Execute_ValueLessThanBase_ReturnsFalseLiteral(string baseValue, string value)
        {
            // arrange
            var sut = CreateGreaterCommand();
            sut.Base = new Literal(baseValue);
            sut.Value = new Literal(value);

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal(Constants.FalseLiteral, literalValue.Value);
        }

        [Theory]
        [InlineData("1976-12-01 12:13:14", "1976-12-01 12:13:15")]
        [InlineData("1976-12-01 12:13:14", "1977-12-01 12:13:14")]
        public void Execute_DateValueGreaterThanBase_ReturnsTrueLiteral(string baseValue, string value)
        {
            // arrange
            var sut = CreateGreaterCommand();
            sut.IsDate = true;
            sut.Base = new Literal(baseValue);
            sut.Value = new Literal(value);

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal(Constants.TrueLiteral, literalValue.Value);
        }

        [Theory]
        [InlineData("1976-12-01 12:13:15", "1976-12-01 12:13:14")]
        [InlineData("1977-12-01 12:13:14", "1976-12-01 12:13:14")]
        public void Execute_DateValueLessThanBase_ReturnsFalseLiteral(string baseValue, string value)
        {
            // arrange
            var sut = CreateGreaterCommand();
            sut.IsDate = true;
            sut.Base = new Literal(baseValue);
            sut.Value = new Literal(value);

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalValue = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal(Constants.FalseLiteral, literalValue.Value);
        }

        private Greater CreateGreaterCommand()
        {
            return new Greater(new ParameterParser());
        }
    }
}
