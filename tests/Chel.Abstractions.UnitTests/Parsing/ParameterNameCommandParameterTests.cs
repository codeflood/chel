using System;
using System.Collections.Generic;
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
        public void Equals_InstancesAreEqual_ReturnsTrue()
        {
            // arrange
            var sut1 = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par");
            var sut2 = new ParameterNameCommandParameter(new SourceLocation(1, 1), "par");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(InstancesNotEqualDataSource))]
        public void Equals_InstancesAreDifferent_ReturnsFalse(ParameterNameCommandParameter instance1, ParameterNameCommandParameter instance2)
        {
            // act
            var result = instance1.Equals(instance2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_InstancesAreEqual_ReturnsSameHashCode()
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

        [Theory]
        [MemberData(nameof(InstancesNotEqualDataSource))]
        public void GetHashCode_InstancesAreNotEqual_ReturnsDifferentHashCodes(ParameterNameCommandParameter instance1, ParameterNameCommandParameter instance2)
        {
            // act
            var hashCode1 = instance1.GetHashCode();
            var hashCode2 = instance2.GetHashCode();

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

        public static IEnumerable<object[]> InstancesNotEqualDataSource()
        {
            yield return new[] {
                new ParameterNameCommandParameter(new SourceLocation(1, 1), "par1"),
                new ParameterNameCommandParameter(new SourceLocation(1, 1), "par2")
            };

            yield return new[] {
                new ParameterNameCommandParameter(new SourceLocation(1, 1), "par1"),
                new ParameterNameCommandParameter(new SourceLocation(2, 1), "par1")
            };
        }
    }
}