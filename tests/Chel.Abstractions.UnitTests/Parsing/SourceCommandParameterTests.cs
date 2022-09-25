using System.Collections.Generic;
using Chel.Abstractions.Parsing;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class SourceCommandParameterTests
    {
        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var location = new SourceLocation(2, 1);

            // act
            var sut = Substitute.ForPartsOf<SourceCommandParameter>(location);

            // assert
            Assert.Equal(location, sut.SourceLocation);
        }

        [Fact]
        public void Equals_InstancesEqual_ReturnsTrue()
        {
            // arrange
            var location1 = new SourceLocation(30, 1);
            var instance1 = Substitute.ForPartsOf<SourceCommandParameter>(location1);

            var location2 = new SourceLocation(30, 1);
            var instance2 = Substitute.ForPartsOf<SourceCommandParameter>(location2);

            // act
            var result = instance1.Equals(instance2);

            // assert
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(InstancesNotEqualDataSource))]
        public void Equals_InstancesNotEqual_ReturnsFalse(SourceCommandParameter instance1, SourceCommandParameter instance2)
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
            var instance1 = Substitute.ForPartsOf<SourceCommandParameter>(location1);

            var location2 = new SourceLocation(30, 1);
            var instance2 = Substitute.ForPartsOf<SourceCommandParameter>(location2);

            // act
            var result1 = instance1.GetHashCode();
            var result2 = instance2.GetHashCode();

            // assert
            Assert.Equal(result1, result2);
        }

        [Theory]
        [MemberData(nameof(InstancesNotEqualDataSource))]
        public void GetHashCode_InstancesNotEqual_ReturnsDifferentCode(SourceCommandParameter instance1, SourceCommandParameter instance2)
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
                Substitute.ForPartsOf<SourceCommandParameter>(new SourceLocation(30, 1)),
                Substitute.ForPartsOf<SourceCommandParameter>(new SourceLocation(30, 1))
            };

            yield return new[] {
                Substitute.ForPartsOf<SourceCommandParameter>(new SourceLocation(30, 1)),
                Substitute.ForPartsOf<SourceCommandParameter>(new SourceLocation(31, 1))
            };
        }
    }
}
