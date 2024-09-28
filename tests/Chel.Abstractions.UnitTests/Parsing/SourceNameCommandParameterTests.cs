using System;
using System.Collections.Generic;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
	public class SourceNameCommandParameterTests
    {
        [Fact]
        public void Ctor_NameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new SourceNameCommandParameter(new SourceLocation(1, 2), null!);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData(" ")]
        public void Ctor_NameIsInvalid_ThrowsException(string name)
        {
            // arrange
            Action sutAction = () => new SourceNameCommandParameter(new SourceLocation(1, 2), name);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var location = new SourceLocation(2, 1);

            // act
            var sut = new SourceNameCommandParameter(location, "name");

            // assert
            Assert.Equal("name", sut.Name);
            Assert.Equal(location, sut.SourceLocation);
        }

        [Fact]
        public void Equals_InstancesEqual_ReturnsTrue()
        {
            // arrange
            var location1 = new SourceLocation(30, 1);
            var instance1 = new SourceNameCommandParameter(location1, "💾");

            var location2 = new SourceLocation(30, 1);
            var instance2 = new SourceNameCommandParameter(location2, "💾");

            // act
            var result = instance1.Equals(instance2);

            // assert
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(InstancesNotEqualDataSource))]
        public void Equals_InstancesNotEqual_ReturnsFalse(SourceNameCommandParameter instance1, SourceNameCommandParameter instance2)
        {
            // act
            var result = instance1.Equals(instance2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_InstancesEqual_ReturnsSameCode()
        {
            // arrange
            var location1 = new SourceLocation(30, 1);
            var instance1 = new SourceNameCommandParameter(location1, "name1");

            var location2 = new SourceLocation(30, 1);
            var instance2 = new SourceNameCommandParameter(location2, "name1");

            // act
            var result1 = instance1.GetHashCode();
            var result2 = instance2.GetHashCode();

            // assert
            Assert.Equal(result1, result2);
        }

        [Theory]
        [MemberData(nameof(InstancesNotEqualDataSource))]
        public void GetHashCode_InstancesNotEqual_ReturnsDifferentCode(SourceNameCommandParameter instance1, SourceNameCommandParameter instance2)
        {
            // act
            var result1 = instance1.GetHashCode();
            var result2 = instance2.GetHashCode();

            // assert
            Assert.NotEqual(result1, result2);
        }

        public static IEnumerable<object[]> InstancesNotEqualDataSource()
        {
            yield return new[] {
                new SourceNameCommandParameter(new SourceLocation(30, 1), "name1"),
                new SourceNameCommandParameter(new SourceLocation(30, 1), "name2")
            };

            yield return new[] {
                new SourceNameCommandParameter(new SourceLocation(30, 1), "name"),
                new SourceNameCommandParameter(new SourceLocation(31, 1), "name")
            };
        }
    }
}
