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
            var builder = new CommandDescriptor.Builder(type, "command");
            builder.AddDescription("description", "en");
            
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
            var builder = new CommandDescriptor.Builder(type, "command");
            builder.AddDescription("description", "en");

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

            var type1WithCommandBuilder = new CommandDescriptor.Builder(type1, "command");
            type1WithCommandBuilder.AddDescription("description", "en");

            var type2WithCommandBuilder = new CommandDescriptor.Builder(type2, "command");
            type2WithCommandBuilder.AddDescription("description", "en");

            var type1WithOtherBuilder = new CommandDescriptor.Builder(type1, "other");
            type1WithOtherBuilder.AddDescription("description", "en");

            var type2WithOtherBuilder = new CommandDescriptor.Builder(type2, "other");
            type2WithOtherBuilder.AddDescription("description", "en");

            yield return new object[] { type2WithCommandBuilder.Build(), type1WithCommandBuilder.Build() };
            yield return new object[] { type1WithOtherBuilder.Build(), type1WithCommandBuilder.Build() };
            yield return new object[] { type2WithOtherBuilder.Build(), type1WithCommandBuilder.Build() };
        }

        [Fact]
        public void GetDescription_CultureNameIsNull_ThrowsException()
        {
            // arrange
            var builder = CreateCommandDescriptorBuilder();
            builder.AddDescription("description", "en");
            var sut = builder.Build();
            Action sutAction = () => sut.GetDescription(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("cultureName", ex.ParamName);
        }

        [Fact]
        public void GetDescription_DescriptionPresent_ReturnsDescription()
        {
            // arrange
            var builder = CreateCommandDescriptorBuilder();
            builder.AddDescription("description", "en");
            var sut = builder.Build();

            // act
            var description = sut.GetDescription("en");

            // assert
            Assert.Equal("description", description);
        }

        [Fact]
        public void GetDescription_DescriptionNotPresent_ReturnsNull()
        {
            // arrange
            var builder = CreateCommandDescriptorBuilder();
            builder.AddDescription("description", "en");
            var sut = builder.Build();

            // act
            var description = sut.GetDescription("fr");

            // assert
            Assert.Null(description);
        }

        [Fact]
        public void GetDescription_LessSpecificDescriptionPresent_ReturnsLessSpecificDescription()
        {
            // arrange
            var builder = CreateCommandDescriptorBuilder();
            builder.AddDescription("description", "en");
            var sut = builder.Build();

            // act
            var description = sut.GetDescription("en-AU");

            // assert
            Assert.Equal("description", description);
        }

        [Theory]
        [InlineData("fr")]
        [InlineData("en-AU")]
        public void GetDescription_OnlyInvariantDescriptionPresent_ReturnsInvariantDescription(string cultureName)
        {
            // arrange
            var builder = CreateCommandDescriptorBuilder();
            builder.AddDescription("description", null);
            var sut = builder.Build();

            // act
            var description = sut.GetDescription(cultureName);

            // assert
            Assert.Equal("description", description);
        }

        private CommandDescriptor.Builder CreateCommandDescriptorBuilder()
        {
            return new CommandDescriptor.Builder(GetType(), "command");
        }
    }
}