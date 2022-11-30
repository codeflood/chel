using System;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class ParameterNameCommandParameterTests
    {
        [Fact]
        public void Ctor_ParameterNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new ParameterNameCommandParameter(new SourceLocation(1, 1), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("parameterName", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Ctor_ParameterNameIsInvalid_ThrowsException(string name)
        {
            // arrange
            Action sutAction = () => new ParameterNameCommandParameter(new SourceLocation(1, 1), name);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("parameterName", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par");

            // assert
            Assert.Equal("par", sut.ParameterName);
        }

        [Fact]
        public void Equals_ParameterNamesAreEqual_ReturnsTrue()
        {
            // arrange
            var sut1 = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par");
            var sut2 = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        //todo: source locations are different

        [Fact]
        public void Equals_ParameterNamesAreDifferent_ReturnsFalse()
        {
            // arrange
            var sut1 = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par1");
            var sut2 = new ParameterNameCommandParameter(new SourceLocation(1, 4), "par2");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ParameterNamesAreEqual_ReturnsSameHashCode()
        {
            // arrange
            var sut1 = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par");
            var sut2 = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par");

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashCode1, hashCode2);
        }

        //todo: source locations are different

        [Fact]
        public void GetHashCode_ParameterNamesAreNotEqual_ReturnsDifferentHashCodes()
        {
            // arrange
            var sut1 = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par1");
            var sut2 = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par2");

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // act, assert
            Assert.NotEqual(hashCode1, hashCode2);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsParameterName()
        {
            // arrange
            var sut = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par");

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("-par", result);
        }
    }
}