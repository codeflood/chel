using System;
using System.Collections.Generic;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class SourceValueCommandParameterTests
    {
        [Fact]
        public void Ctor_ValueIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new SourceValueCommandParameter(new SourceLocation(1, 2), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var value = new VariableReference("something");
            var location = new SourceLocation(2, 1);

            // act
            var sut = new SourceValueCommandParameter(location, value);

            // assert
            Assert.Equal(value, sut.Value);
            Assert.Equal(location, sut.SourceLocation);
        }

        [Fact]
        public void Equals_InstancesEqual_ReturnsTrue()
        {
            // arrange
            var value1 = new Literal("lit1");
            var location1 = new SourceLocation(30, 1);
            var instance1 = new SourceValueCommandParameter(location1, value1);

            var value2 = new Literal("lit1");
            var location2 = new SourceLocation(30, 1);
            var instance2 = new SourceValueCommandParameter(location2, value2);

            // act
            var result = instance1.Equals(instance2);

            // assert
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(InstancesNotEqualDataSource))]
        public void Equals_InstancesNotEqual_ReturnsFalse(SourceValueCommandParameter instance1, SourceValueCommandParameter instance2)
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
            var value1 = new Literal("lit1");
            var location1 = new SourceLocation(30, 1);
            var instance1 = new SourceValueCommandParameter(location1, value1);

            var value2 = new Literal("lit1");
            var location2 = new SourceLocation(30, 1);
            var instance2 = new SourceValueCommandParameter(location2, value2);

            // act
            var result1 = instance1.GetHashCode();
            var result2 = instance2.GetHashCode();

            // assert
            Assert.Equal(result1, result2);
        }

        [Theory]
        [MemberData(nameof(InstancesNotEqualDataSource))]
        public void GetHashCode_InstancesNotEqual_ReturnsDifferentCode(SourceValueCommandParameter instance1, SourceValueCommandParameter instance2)
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
                new SourceValueCommandParameter(new SourceLocation(30, 1), new Literal("lit1")),
                new SourceValueCommandParameter(new SourceLocation(30, 1), new Literal("lit2"))
            };

            yield return new[] {
                new SourceValueCommandParameter(new SourceLocation(30, 1), new Literal("lit1")),
                new SourceValueCommandParameter(new SourceLocation(31, 1), new Literal("lit1"))
            };
        }
    }
}
