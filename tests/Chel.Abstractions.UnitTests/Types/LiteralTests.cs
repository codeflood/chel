using System;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests.Types
{
    public class LiteralTests
    {
        [Fact]
        public void Ctor_ValueIsNull_ThrowsExceptino()
        {
            // arrange
            Action sutAction = () => new Literal(null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void Ctor_ValueIsEmpty_DoesNotThrow()
        {
            var sut = new Literal("");
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange, act
            var sut = new Literal("val");

            // assert
            Assert.Equal("val", sut.Value);
        }

        [Fact]
        public void Equals_ValuesAreEqual_ReturnsTrue()
        {
            // arrange
            var sut1 = new Literal("val1");
            var sut2 = new Literal("val1");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ValuesAreDifferent_ReturnsFalse()
        {
            // arrange
            var sut1 = new Literal("val1");
            var sut2 = new Literal("val2");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ValuesAreEqual_ReturnsSameHashCode()
        {
            // arrange
            var sut1 = new Literal("val1");
            var sut2 = new Literal("val1");

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
            var sut1 = new Literal("val1");
            var sut2 = new Literal("val2");

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // act, assert
            Assert.NotEqual(hashCode1, hashCode2);
        }

        [Fact]
        public void ToString_WhenCalled_ReturnsValue()
        {
            // arrange
            var sut = new Literal("val");

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("val", result);
        }
    }
}
