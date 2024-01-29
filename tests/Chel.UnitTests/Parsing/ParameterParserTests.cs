using System;
using Chel.Abstractions.Types;
using Chel.Parsing;
using Xunit;

namespace Chel.UnitTests.Parsing
{
    public class ParameterParserTests
    {
        private static ParameterParser _sut = new ParameterParser();

        [Fact]
        public void ParseDouble_InputIsNotLiteral_ReturnsErrorResult()
        {
            // act
            var input = new List(new[] { new Literal("2") });
            var result = _sut.ParseDouble(input, "param");

            // assert
            Assert.True(result.HasError);
            Assert.Contains("param", result.ErrorMessage);
        }

        [Fact]
        public void ParseDouble_InputIsNotDouble_ReturnsErrorResult()
        {
            // act
            var input = new Literal("a");
            var result = _sut.ParseDouble(input, "param");

            // assert
            Assert.True(result.HasError);
            Assert.Contains("a", result.ErrorMessage);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(2.2)]
        public void ParseDouble_InputIsDouble_ReturnsDoubleResult(double inputValue)
        {
            // act
            var input = new Literal(inputValue.ToString());
            var result = _sut.ParseDouble(input, "param");

            // assert
            Assert.False(result.HasError);
            Assert.Equal(inputValue, result.Value);
        }

        [Fact]
        public void ParseDateTime_InputIsNotLiteral_ReturnsErrorResult()
        {
            // act
            var input = new List(new[] { new Literal("2") });
            var result = _sut.ParseDateTime(input, "param");

            // assert
            Assert.True(result.HasError);
            Assert.Contains("param", result.ErrorMessage);
        }

        [Fact]
        public void ParseDateTime_InputIsNotDouble_ReturnsErrorResult()
        {
            // act
            var input = new Literal("a");
            var result = _sut.ParseDateTime(input, "param");

            // assert
            Assert.True(result.HasError);
            Assert.Contains("a", result.ErrorMessage);
        }

        [Fact]
        public void ParseDateTime_InputIsDouble_ReturnsDoubleResult()
        {
            // act
            var date = new DateTime(2013, 10, 01, 13, 12, 14);
            var input = new Literal(date.ToString());
            var result = _sut.ParseDateTime(input, "param");

            // assert
            Assert.False(result.HasError);
            Assert.Equal(date, result.Value);
        }

        [Fact]
        public void ParseGuid_InputIsNotLiteral_ReturnsErrorResult()
        {
            // act
            var input = new List(new[] { new Literal("2") });
            var result = _sut.ParseGuid(input, "param");

            // assert
            Assert.True(result.HasError);
            Assert.Contains("param", result.ErrorMessage);
        }

        [Fact]
        public void ParseGuid_InputIsNotDouble_ReturnsErrorResult()
        {
            // act
            var input = new Literal("a");
            var result = _sut.ParseGuid(input, "param");

            // assert
            Assert.True(result.HasError);
            Assert.Contains("a", result.ErrorMessage);
        }

        [Fact]
        public void ParseGuid_InputIsDouble_ReturnsDoubleResult()
        {
            // act
            var guid = Guid.NewGuid();
            var input = new Literal(guid.ToString());
            var result = _sut.ParseGuid(input, "param");

            // assert
            Assert.False(result.HasError);
            Assert.Equal(guid, result.Value);
        }
    }
}
