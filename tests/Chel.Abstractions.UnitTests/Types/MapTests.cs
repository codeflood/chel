using System.Collections.Generic;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests.Types
{
    public class MapTests
    {
        [Fact]
        public void Ctor_EntriesIsNull_SetsPropertyToEmptyCollectionm()
        {
            // arrange, act
            var sut = new Map(null);

            // assert
            Assert.Empty(sut.Entries);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var expected = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("val1") },
                { "key2", new Literal("val2") }
            };

            // act
            var sut = new Map(expected);

            // assert
            Assert.Equal(expected, sut.Entries);
        }

        [Fact]
        public void Equals_EntriesAreBothEmpty_ReturnsTrue()
        {
            // arrange
            var sut1 = new Map(null);
            var sut2 = new Map(null);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_EntriesBothContainEqualElements_ReturnsTrue()
        {
            // arrange
            var d1 = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("val1") },
                { "key2", new Literal("val2") }
            };

            var d2 = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("val1") },
                { "key2", new Literal("val2") }
            };

            var sut1 = new Map(d1);
            var sut2 = new Map(d2);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_DifferentEntries_ReturnsFalse()
        {
            // arrange
            var d1 = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("val1") },
                { "key2", new Literal("val2") }
            };

            var d2 = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("val1") },
                { "key3", new Literal("val2") }
            };

            var sut1 = new Map(d1);
            var sut2 = new Map(d2);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_EntriesAreEqual_ReturnsSameHashCode()
        {
            // arrange
            var d1 = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("val1") },
                { "key2", new Literal("val2") }
            };

            var d2 = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("val1") },
                { "key2", new Literal("val2") }
            };

            var sut1 = new Map(d1);
            var sut2 = new Map(d2);

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_EntriesAreDifferent_ReturnsDifferentHashCodes()
        {
            // arrange
            var d1 = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("val1") },
                { "key2", new Literal("val2") }
            };

            var d2 = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("val1") },
                { "key2", new Literal("val3") }
            };

            var sut1 = new Map(d1);
            var sut2 = new Map(d2);

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.NotEqual(hashCode1, hashCode2);
        }

        [Fact]
        public void ToString_MapIsEmpty_ReturnsEmptyMap()
        {
            // arrange
            var sut = new Map(null);

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("{ }", result);
        }

        [Fact]
        public void ToString_MapHasEntries_ReturnsFormattedMap()
        {
            // arrange
            var data = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("val1") },
                { "key2", new Literal("val2") }
            };

            var sut = new Map(data);

            // act
            var result = sut.ToString();

            // assert
            var expected = @"{ key1: val1 key2: val2 }";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToString_MapHasEntriesWithSpaces_ReturnsFormattedMap()
        {
            // arrange
            var data = new Dictionary<string, ICommandParameter> {
                { "key1", new Literal("value 1 with spaces") },
                { "key2", new Literal("value 2 with spaces") }
            };

            var sut = new Map(data);

            // act
            var result = sut.ToString();

            // assert
            var expected = @"{
  key1: (value 1 with spaces)
  key2: (value 2 with spaces)
}";

            Assert.Equal(expected, result, ignoreLineEndingDifferences: true);
        }

        [Fact]
        public void ToString_MapHasList_ReturnsFormattedList()
        {
            // arrange
            var data = new Dictionary<string, ICommandParameter> {
                { "a", new List(new[] { new Literal("1"), new Literal("2")})},
                { "b", new List(new[] { new Literal("3"), new Literal("4")})}
            };

            var sut = new Map(data);

            // act
            var result = sut.ToString();

            // assert
            var expected = @"{ a: [ 1 2 ] b: [ 3 4 ] }";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToString_MapHasListWithLongerValues_ReturnsFormattedList()
        {
            // arrange
            var data = new Dictionary<string, ICommandParameter> {
                { "key1", new List(new[] { new Literal("val1"), new Literal("val2")})},
                { "key2", new List(new[] { new Literal("val3"), new Literal("val4")})}
            };

            var sut = new Map(data);

            // act
            var result = sut.ToString();

            // assert
            var expected = @"{
  key1: [ val1 val2 ]
  key2: [ val3 val4 ]
}";

            Assert.Equal(expected, result, ignoreLineEndingDifferences: true);
        }

        [Fact]
        public void ToString_MapHasMap_ReturnsFormattedList()
        {
            // arrange
            var innerMap = new Map(new Dictionary<string, ICommandParameter> {
                { "a", new Literal("b") },
                { "c", new Literal("d") },
            });

            var data = new Dictionary<string, ICommandParameter> {
                { "out", innerMap }
            };

            var sut = new Map(data);

            // act
            var result = sut.ToString();

            // assert
            var expected = @"{
  out: { a: b c: d }
}";

            Assert.Equal(expected, result, ignoreLineEndingDifferences: true);
        }

        [Fact]
        public void Entries_UseWrongCaseForKey_ValueIsReturned()
        {
            // arrange
            var map = new Map(new Dictionary<string, ICommandParameter> {
                { "a", new Literal("b") }
            });

            // act
            var result = map.Entries["A"];

            // assert
            Assert.Equal("b", ((Literal)result).Value);
        }
    }
}