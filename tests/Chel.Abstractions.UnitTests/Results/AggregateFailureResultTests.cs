using Xunit;
using Chel.Abstractions.Results;
using System;

namespace Chel.Abstractions.UnitTests.Results
{
    public class AggregateFailureResultTests
    {
        [Fact]
        public void Ctor_InnerResultsIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new AggregateFailureResult(null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("innerResults", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsSuccessToFalse()
        {
            // act
            var result = new FailureResult(new(1, 1), "message");
            var sut = new AggregateFailureResult(new[] { result });

            // assert
            Assert.False(sut.Success);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // assert
            var result1 = new FailureResult(new(1, 1), "message1");
            var result2 = new FailureResult(new(2, 1), "message2");

            // act
            var sut = new AggregateFailureResult(new[] { result1, result2 });

            // assert
            Assert.Equal(new[] { result1, result2 }, sut.InnerResults);
        }

        [Fact]
        public void ToString_MultipleInnerResults_ReturnsEachResult()
        {
            // arrange
            var result1 = new FailureResult(new(1, 1), "message1");
            var result2 = new FailureResult(new(2, 2), "message2");
            var sut = new AggregateFailureResult(new[] { result1, result2 });

            // act
            var result = sut.ToString();
            var lines = result.Split(Environment.NewLine);

            // assert
            Assert.Equal(2, lines.Length);
            Assert.Equal("ERROR (line 1, character 1): message1", lines[0]);
            Assert.Equal("ERROR (line 2, character 2): message2", lines[1]);
        }
    }
}
