using System;
using System.Collections.Generic;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandDescriptorTests
    {
        [Fact]
        public void Ctor_ImplementingTypeIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor(null, "something");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("implementingType", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor(GetType(), null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
        }

        [Fact]
        public void Ctor_CommandNameIsEmpty_ThrowsException()
        {
            // arrange
            Action sutAction = () => new CommandDescriptor(GetType(), "");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("commandName", ex.ParamName);
            Assert.Contains("commandName cannot be empty", ex.Message);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var type = GetType();

            // act
            var sut = new CommandDescriptor(type, "command");

            // assert
            Assert.Equal(type, sut.ImplementingType);
            Assert.Equal("command", sut.CommandName);
        }

        [Fact]
        public void Equals_DescriptorsAreEqual_ReturnsTrue()
        {
            // arrange
            var type = GetType();
            var sut1 = new CommandDescriptor(type, "command");
            var sut2 = new CommandDescriptor(type, "command");

            // act
            var result = sut1.Equals(sut2);

            // assert
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
            var sut1 = new CommandDescriptor(type, "command");
            var sut2 = new CommandDescriptor(type, "command");

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

            yield return new object[] { new CommandDescriptor(type2, "command"), new CommandDescriptor(type1, "command") };
            yield return new object[] { new CommandDescriptor(type1, "other"), new CommandDescriptor(type1, "command") };
            yield return new object[] { new CommandDescriptor(type2, "other"), new CommandDescriptor(type1, "command") };
        }
    }
}