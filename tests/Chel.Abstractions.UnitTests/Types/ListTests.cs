using System.Collections.Generic;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests.Types
{
    public class ListTests
    {
        [Fact]
        public void Ctor_ParametersIsNull_SetsPropertyToEmptyCollection()
        {
            // arrange, act
            var sut = new List(null);

            // assert
            Assert.Empty(sut.Values);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var val1 = new Literal("val1");
            var val2 = new Literal("val2");

            // act
            var sut = new List(new List<ChelType> { val1, val2 });

            // assert
            Assert.Equal(new[] { val1, val2 }, sut.Values);
        }

        [Fact]
        public void Equals_ValuesAreBothEmpty_ReturnsTrue()
        {
            // arrange
            var sut1 = new List(null);
            var sut2 = new List(null);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ValuesBothContainEqualElements_ReturnsTrue()
        {
            // arrange
            var param11 = new Literal("val1");
            var param21 = new Literal("val1");

            var param12 = new Literal("val2");
            var param22 = new Literal("val2");

            var sut1 = new List(new[] { param11, param12 });
            var sut2 = new List(new[] { param21, param22 });

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_SameValuesOutOfOrder_ReturnsFalse()
        {
            // arrange
            var param11 = new Literal("val1");
            var param21 = new Literal("val1");

            var param12 = new Literal("val2");
            var param22 = new Literal("val2");

            var sut1 = new List(new[] { param11, param12 });
            var sut2 = new List(new[] { param22, param21 });

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            // arrange
            var param1 = new Literal("val");
            var param2 = new Literal("other");

            var sut1 = new List(new[] { param1 });
            var sut2 = new List(new[] { param2 });

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ValuesAreEqual_ReturnsSameHashCode()
        {
            // arrange
            var param11 = new Literal("val1");
            var param21 = new Literal("val1");

            var param12 = new Literal("val2");
            var param22 = new Literal("val2");

            var sut1 = new List(new[] { param11, param12 });
            var sut2 = new List(new[] { param21, param22 });

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_ValuesAreDifferent_ReturnsDifferentHashCodes()
        {
            // arrange
            var param11 = new Literal("val1");
            var param21 = new Literal("val1");

            var param12 = new Literal("val2");
            var param22 = new Literal("val3");

            var sut1 = new List(new[] { param11, param12 });
            var sut2 = new List(new[] { param21, param22 });

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.NotEqual(hashCode1, hashCode2);
        }

        [Fact]
        public void ToString_ListIsEmpty_ReturnsEmptyList()
        {
            // arrange
            var sut = new List(null);

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("[ ]", result);
        }

        [Fact]
        public void ToString_ListHasElements_IncludesAllElementValues()
        {
            // arrange
            var param1 = new Literal("val1");
            var param2 = new Literal("val2");

            var sut = new List(new[] { param1, param2 });

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("[ val1 val2 ]", result);
        }
    }
}
