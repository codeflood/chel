using System;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class SourceLocationTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Ctor_LineNumberIsInvalid_ThrowsException(int lineNumber)
        {
            // arrange
            Action sutAction = () => new SourceLocation(lineNumber, 1);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("lineNumber", ex.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Ctor_CharacterNumberIsInvalid_ThrowsException(int characterNumber)
        {
            // arrange
            Action sutAction = () => new SourceLocation(1, characterNumber);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("characterNumber", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new SourceLocation(3, 4);

            // assert
            Assert.Equal(3, sut.LineNumber);
            Assert.Equal(4, sut.CharacterNumber);
        }

        [Fact]
        public void Equals_ValuesAreEqual_ReturnsTrue()
        {
            // arrange
            var sut1 = new SourceLocation(3, 6);
            var sut2 = new SourceLocation(3, 6);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ValuesAreNotEqual_ReturnsFalse()
        {
            // arrange
            var sut1 = new SourceLocation(3, 6);
            var sut2 = new SourceLocation(4, 6);

            // act
            var result = sut1 == sut2;

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ValueIsNull_ReturnsFalse()
        {
            // arrange
            var sut = new SourceLocation(3, 6);

            // act
            var result = sut.Equals(null);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ValuesAreEqual_ReturnsSameHashCode()
        {
            // arrange
            var sut1 = new SourceLocation(3, 6);
            var sut2 = new SourceLocation(3, 6);

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut1.GetHashCode();

            // assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_ValuesAreNotEqual_ReturnsDifferentHashCodes()
        {
            // arrange
            var sut1 = new SourceLocation(3, 6);
            var sut2 = new SourceLocation(3, 7);

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // act, assert
            Assert.NotEqual(hashCode1, hashCode2);
        }
    }
}