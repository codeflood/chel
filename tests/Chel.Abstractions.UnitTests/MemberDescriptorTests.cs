using System;
using NSubstitute;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class MemberDescriptorTests
    {
        [Fact]
        public void GetDescription_CultureNameIsNull_ThrowsException()
        {
            // arrange
            var builder = CreateMemberDescriptorBuilder();
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
            var builder = CreateMemberDescriptorBuilder();
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
            var builder = CreateMemberDescriptorBuilder();
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
            var builder = CreateMemberDescriptorBuilder();
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
            var builder = CreateMemberDescriptorBuilder();
            builder.AddDescription("description", null);
            var sut = builder.Build();

            // act
            var description = sut.GetDescription(cultureName);

            // assert
            Assert.Equal("description", description);
        }

        private MemberDescriptor.MemberDescriptorBuilder<TestMemberDescriptor> CreateMemberDescriptorBuilder()
        {
            return Substitute.ForPartsOf<MemberDescriptor.MemberDescriptorBuilder<TestMemberDescriptor>>();
        }
    }
}