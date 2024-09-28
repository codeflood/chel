using System;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class ParameterParsingResultTests
    {
        [Fact]
        public void Ctor_NullErrorMessageProvided_ThrowsException()
        {
            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(() => new ParameterParsingResult<int>(null!));
            Assert.Equal("errorMessage", ex.ParamName);
        }
        
        [Fact]
        public void Ctor_ErrorMessageProvided_SetsErrorMessage()
        {
            // act
            var sut = new ParameterParsingResult<int>("bad");

            // assert
            Assert.Equal("bad", sut.ErrorMessage);
        }

        [Fact]
        public void Ctor_ErrorMessageProvided_HasErrorIsTrue()
        {
            // act
            var sut = new ParameterParsingResult<int>("bad");

            // assert
            Assert.True(sut.HasError);
        }

        [Fact]
        public void Ctor_ValueProvided_SetsValueProperty()
        {
            // act
            var sut = new ParameterParsingResult<int>(42);

            // assert
            Assert.Equal(42, sut.Value);
        }

        [Fact]
        public void Ctor_ValueProvided_HasErrorIsFalse()
        {
            // act
            var sut = new ParameterParsingResult<int>(42);

            // assert
            Assert.False(sut.HasError);
        }
    }
}
