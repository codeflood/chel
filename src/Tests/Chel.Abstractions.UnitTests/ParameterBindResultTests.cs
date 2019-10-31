using System;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class ParameterBindResultTests
    {
        [Fact]
        public void AddError_ErrorIsNull_ThrowsException()
        {
            // arrange
            var sut = new ParameterBindResult();
            Action sutAction = () => sut.AddError(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("error", ex.ParamName);
        }

        [Fact]
        public void AddError_ErrorIsEmpty_ThrowsException()
        {
            // arrange
            var sut = new ParameterBindResult();
            Action sutAction = () => sut.AddError("");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("error", ex.ParamName);
            Assert.Contains("error cannot be empty", ex.Message);
        }

        [Fact]
        public void Success_NoErrorsAdded_ReturnsTrue()
        {
            // arrange
            var sut = new ParameterBindResult();

            // act
            var result = sut.Success;

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Success_ErrorsAdded_ReturnsFalse()
        {
            // arrange
            var sut = new ParameterBindResult();
            sut.AddError("error");

            // act
            var result = sut.Success;

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Errors_NoErrorsAdded_ReturnsEmptyCollection()
        {
            // arrange
            var sut = new ParameterBindResult();

            // act
            var result = sut.Errors;

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public void Errors_ErrorsAdded_ReturnsErrors()
        {
            // arrange
            var sut = new ParameterBindResult();
            sut.AddError("error1");
            sut.AddError("error2");

            // act
            var result = sut.Errors;

            // assert
            Assert.Equal(new[]{ "error1", "error2" }, result);
        }
    }
}