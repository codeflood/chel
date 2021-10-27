using System;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class SpecialTokenTests
    {
        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var location = new SourceLocation(2, 3);

            // act
            var sut = new SpecialToken(location, SpecialTokenType.BlockStart);

            // assert
            Assert.Equal(2, sut.Location.LineNumber);
            Assert.Equal(3, sut.Location.CharacterNumber);
            Assert.Equal(SpecialTokenType.BlockStart, sut.Type);
        }

        [Fact]
        public void Equals_ValuesAreEqual_ReturnsTrue()
        {
            // arrange
            var sut1 = new SpecialToken(new SourceLocation(3, 6), SpecialTokenType.BlockEnd);
            var sut2 = new SpecialToken(new SourceLocation(3, 6), SpecialTokenType.BlockEnd);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_LocationIsDifferent_ReturnsFalse()
        {
            // arrange
            var sut1 = new SpecialToken(new SourceLocation(3, 6), SpecialTokenType.BlockEnd);
            var sut2 = new SpecialToken(new SourceLocation(4, 6), SpecialTokenType.BlockEnd);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_TypeIsDifferent_ReturnsFalse()
        {
            // arrange
            var sut1 = new SpecialToken(new SourceLocation(3, 6), SpecialTokenType.BlockEnd);
            var sut2 = new SpecialToken(new SourceLocation(3, 6), SpecialTokenType.BlockStart);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ValuesAreEqual_ReturnsSameHashCode()
        {
            // arrange
            var sut1 = new SpecialToken(new SourceLocation(3, 6), SpecialTokenType.BlockEnd);
            var sut2 = new SpecialToken(new SourceLocation(3, 6), SpecialTokenType.BlockEnd);

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_ValuesAreNotEqual_ReturnsDifferentHashCodes()
        {
            // arrange
            var sut1 = new SpecialToken(new SourceLocation(3, 6), SpecialTokenType.BlockEnd);
            var sut2 = new SpecialToken(new SourceLocation(3, 6), SpecialTokenType.BlockStart);

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // act, assert
            Assert.NotEqual(hashCode1, hashCode2);
        }
    }
}