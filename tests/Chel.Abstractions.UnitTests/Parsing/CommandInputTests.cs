using System;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class CommandInputTests
    {
        private readonly SourceLocation SourceLocation = new SourceLocation(2, 42);

        [Fact]
        public void Equals_ObjectIsNull_ReturnsFalse()
        {
            // arrange
            var sut = new CommandInput.Builder(SourceLocation, "cmd").Build();

            // act
            var result = sut.Equals(null);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ObjectWrongType_ReturnsFalse()
        {
            // arrange
            var sut = new CommandInput.Builder(SourceLocation, "cmd").Build();

            // act
            var result = sut.Equals("wrong");

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ObjectsAreEqual_ReturnsTrue()
        {
            // arrange
            Func<CommandInput> factory = () => {
                var subcommand = new CommandInput.Builder(SourceLocation, "subcmd").Build();

                var builder = new CommandInput.Builder(SourceLocation, "cmd");
                builder.AddParameter(new Literal("val"));
                builder.AddParameter(new List(new[]{ new Literal("lv1"), new Literal("lv2") }));
                builder.AddParameter(subcommand);
                return builder.Build();
            };

            var sut1 = factory.Invoke();
            var sut2 = factory.Invoke();

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ObjectsAreDifferent_ReturnsFalse()
        {
            // arrange
            var builder1 = new CommandInput.Builder(SourceLocation, "cmd");
            builder1.AddParameter(new Literal("val"));
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(SourceLocation, "cmd");
            var sut2 = builder2.Build();

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ObjectsAreEqual_HashCodesAreEqual()
        {
            // arrange
            var builder1 = new CommandInput.Builder(SourceLocation, "cmd");
            builder1.AddParameter(new Literal("val"));
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(SourceLocation, "cmd");
            builder2.AddParameter(new Literal("val"));
            var sut2 = builder2.Build();

            // act
            var result1 = sut1.GetHashCode();
            var result2 = sut2.GetHashCode();

            // assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void GetHashCode_ObjectsAreDifferent_HashCodesAreDifferent()
        {
            // arrange
            var builder1 = new CommandInput.Builder(SourceLocation, "cmd");
            builder1.AddParameter(new Literal("val"));
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(SourceLocation, "cmd");
            var sut2 = builder2.Build();

            // act
            var result1 = sut1.GetHashCode();
            var result2 = sut2.GetHashCode();

            // assert
            Assert.NotEqual(result1, result2);
        }
    }
}