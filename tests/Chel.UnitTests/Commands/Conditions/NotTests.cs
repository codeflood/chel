using System.Collections.Generic;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Commands.Conditions;
using Xunit;

namespace Chel.UnitTests.Commands.Conditions
{
    public class NotTests
    {
        [Fact]
        public void Execute_ValueIsTrue_ReturnsFalse()
        {
            // arrange
            var sut = new Not();
            sut.Value = true;

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalResult = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal("false", literalResult.Value);
        }

        [Fact]
        public void Execute_ValueIsFalse_ReturnsTrue()
        {
            // arrange
            var sut = new Not();
            sut.Value = false;

            // act
            var result = sut.Execute();

            // assert
            var valueResult = Assert.IsType<ValueResult>(result);
            var literalResult = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal("true", literalResult.Value);
        }
    }
}
