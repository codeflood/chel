using System;
using System.Collections.Generic;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandDescriptorTests
    {
        [Fact]
        public void Equals_DescriptorsAreEqual_ReturnsTrue()
        {
            // arrange
            var type = GetType();
            var builder = new CommandDescriptor.Builder(type, "command", "description");
            var sut1 = builder.Build();
            var sut2 = builder.Build();

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.NotSame(sut1, sut2);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(DescriptorsAreDifferentDataSource))]
        public void Equals_DescriptorsAreDifferent_ReturnsFalse(CommandDescriptor sut1, CommandDescriptor sut2)
        {
            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_DescriptorsAreSame_HashCodesAreSame()
        {
            // arrange
            var type = GetType();
            var builder = new CommandDescriptor.Builder(type, "command", "description");
            var sut1 = builder.Build();
            var sut2 = builder.Build();

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashcode1, hashcode2);
        }

        [Theory]
        [MemberData(nameof(DescriptorsAreDifferentDataSource))]
        public void GetHashCode_DescriptorsAreDifferent_CodesAreDifferent(CommandDescriptor sut1, CommandDescriptor sut2)
        {
            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.NotEqual(hashcode1, hashcode2);
        }

        public static IEnumerable<object[]> DescriptorsAreDifferentDataSource()
        {
            var type1 = typeof(CommandDescriptorTests);
            var type2 = typeof(CommandDescriptor);

            var type1WithCommandBuilder = new CommandDescriptor.Builder(type1, "command", "description");
            var type2WithCommandBuilder = new CommandDescriptor.Builder(type2, "command", "description");
            var type1WithOtherBuilder = new CommandDescriptor.Builder(type1, "other", "description");
            var type2WithOtherBuilder = new CommandDescriptor.Builder(type2, "other", "description");

            yield return new object[] { type2WithCommandBuilder.Build(), type1WithCommandBuilder.Build() };
            yield return new object[] { type1WithOtherBuilder.Build(), type1WithCommandBuilder.Build() };
            yield return new object[] { type2WithOtherBuilder.Build(), type1WithCommandBuilder.Build() };
        }
    }
}