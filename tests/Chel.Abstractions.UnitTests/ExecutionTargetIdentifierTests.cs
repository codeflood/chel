using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class ExecutionTargetIdentifierTests
    {
        [Fact]
        public void Ctor_NameIsNull_ThrowsException()
        {
            Action sutAction = () => new ExecutionTargetIdentifier("mod", null);
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void ToString_ModuleIsNull_ReturnsString()
        {
            var sut = new ExecutionTargetIdentifier(null, "name");
            Assert.Equal("name", sut.ToString());
        }

        [Fact]
        public void ToString_ModuleAndNameProvided_ReturnsString()
        {
            var sut = new ExecutionTargetIdentifier("module", "name");
            Assert.Equal("module:name", sut.ToString());
        }

        [Fact]
        public void Equals_OtherInstanceIsNull_ReturnsFalse()
        {
            var sut = new ExecutionTargetIdentifier(null, "a");
            var result = sut.Equals(null);
            Assert.False(result);
        }

        [Fact]
        public void Equals_OtherInstanceNotCorrectType_ReturnsFalse()
        {
            var sut = new ExecutionTargetIdentifier(null, "a");
            var result = sut.Equals("cmd");
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(EqualInstancesDataSource))]
        public void Equals_InstancesAreEqual_ReturnsTrue(ExecutionTargetIdentifier a, ExecutionTargetIdentifier b)
        {
            var equal = a.Equals(b);
            Assert.True(equal);
        }

        [Theory]
        [MemberData(nameof(EqualInstancesDataSource))]
        public void GetHashCode_InstancesAreEqual_ReturnsSameCode(ExecutionTargetIdentifier a, ExecutionTargetIdentifier b)
        {
            var result1 = a.GetHashCode();
            var result2 = b.GetHashCode();
            Assert.Equal(result1, result2);
        }

        public static IEnumerable<object[]> EqualInstancesDataSource()
        {
            yield return new object[] {
                new ExecutionTargetIdentifier(null, "a"),
                new ExecutionTargetIdentifier(null, "a")
            };

            yield return new object[] {
                new ExecutionTargetIdentifier("m", "a"),
                new ExecutionTargetIdentifier("m", "a")
            };

            yield return new object[] {
                new ExecutionTargetIdentifier("m", "a"),
                new ExecutionTargetIdentifier("M", "A")
            };
        }

        [Theory]
        [MemberData(nameof(EqualNotInstancesDataSource))]
        public void Equals_InstancesAreNotEqual_ReturnsFalse(ExecutionTargetIdentifier a, ExecutionTargetIdentifier b)
        {
            var equal = a.Equals(b);
            Assert.False(equal);
        }

        [Theory]
        [MemberData(nameof(EqualNotInstancesDataSource))]
        public void GetHashCode_InstancesAreNotEqual_ReturnsDifferentCodes(ExecutionTargetIdentifier a, ExecutionTargetIdentifier b)
        {
            var result1 = a.GetHashCode();
            var result2 = b.GetHashCode();
            Assert.NotEqual(result1, result2);
        }

        public static IEnumerable<object[]> EqualNotInstancesDataSource()
        {
            yield return new object[] {
                new ExecutionTargetIdentifier(null, "a"),
                new ExecutionTargetIdentifier(null, "b")
            };

            yield return new object[] {
                new ExecutionTargetIdentifier("m", "a"),
                new ExecutionTargetIdentifier("m", "b")
            };

            yield return new object[] {
                new ExecutionTargetIdentifier("m", "a"),
                new ExecutionTargetIdentifier("mb", "a")
            };
        }
    }
}
