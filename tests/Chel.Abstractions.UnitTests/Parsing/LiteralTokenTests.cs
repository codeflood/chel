using System;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class LiteralTokenTests
    {
        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var location = new SourceLocation(2, 3);

            // act
            var sut = new LiteralToken(location, 'b');

            // assert
            Assert.Equal(2, sut.Location.LineNumber);
            Assert.Equal(3, sut.Location.CharacterNumber);
            Assert.Equal('b', sut.Value);
        }

        [Fact]
        public void Equals_ValuesAreEqual_ReturnsTrue()
        {
            // arrange
            var sut1 = new LiteralToken(new SourceLocation(3, 6), 'a');
            var sut2 = new LiteralToken(new SourceLocation(3, 6), 'a');

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_LocationIsDifferent_ReturnsFalse()
        {
            // arrange
            var sut1 = new LiteralToken(new SourceLocation(3, 6), 'a');
            var sut2 = new LiteralToken(new SourceLocation(3, 8), 'a');

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ValueIsDifferent_ReturnsFalse()
        {
            // arrange
            var sut1 = new LiteralToken(new SourceLocation(3, 6), 'a');
            var sut2 = new LiteralToken(new SourceLocation(3, 6), 'b');

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ValuesAreEqual_ReturnsSameHashCode()
        {
            // arrange
            var sut1 = new LiteralToken(new SourceLocation(3, 6), 'a');
            var sut2 = new LiteralToken(new SourceLocation(3, 6), 'a');

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
            var sut1 = new LiteralToken(new SourceLocation(3, 6), 'a');
            var sut2 = new LiteralToken(new SourceLocation(3, 6), 'b');

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // act, assert
            Assert.NotEqual(hashCode1, hashCode2);
        }
    }
}