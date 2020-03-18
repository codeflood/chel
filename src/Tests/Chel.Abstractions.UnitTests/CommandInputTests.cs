using System;
using Chel.Abstractions;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class CommandInputTests
    {
        [Fact]
        public void Equals_CommandsAreSame_ReturnsTrue()
        {
            // arrange
            var sut1 = CreateCommandInput(2, "command");
            var sut2 = CreateCommandInput(2, "command");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "command", 2, "command")]
        [InlineData(1, "command", 1, "other")]
        public void Equals_CommandsAreDifferent_ReturnsFalse(int sourceLine1, string commandName1, int sourceLine2, string commandName2)
        {
            // arrange
            var sut1 = CreateCommandInput(sourceLine1, commandName1);
            var sut2 = CreateCommandInput(sourceLine2, commandName2);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_CommandNamesCasingDifferent_ReturnsTrue()
        {
            // arrange
            var sut1 = CreateCommandInput(4, "command");
            var sut2 = CreateCommandInput(4, "COMMAND");

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_NumberedParametersAreDifferent_ReturnsFalse()
        {
            // arrange
            var builder1 = new CommandInput.Builder(1, "command");
            builder1.AddNumberedParameter("p1");
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(1, "command");
            builder2.AddNumberedParameter("p2");
            var sut2 = builder2.Build();

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_NamedParametersAreDifferent_ReturnsFalse()
        {
            // arrange
            var builder1 = new CommandInput.Builder(1, "command");
            builder1.AddNamedParameter("name1", "value");
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(1, "command");
            builder2.AddNamedParameter("name2", "value");
            var sut2 = builder2.Build();

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_NamedParametersValuesAreDifferent_ReturnsFalse()
        {
            // arrange
            var builder1 = new CommandInput.Builder(1, "command");
            builder1.AddNamedParameter("name", "value1");
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(1, "command");
            builder2.AddNamedParameter("name", "value2");
            var sut2 = builder2.Build();

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_NamedParametersAreEqual_ReturnsTrue()
        {
            // arrange
            var builder1 = new CommandInput.Builder(1, "command");
            builder1.AddNamedParameter("name", "value");
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(1, "command");
            builder2.AddNamedParameter("name", "value");
            var sut2 = builder1.Build();

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_DifferentCasingNamedParametersAreEqual_ReturnsTrue()
        {
            // arrange
            var builder1 = new CommandInput.Builder(1, "command");
            builder1.AddNamedParameter("name", "value");
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(1, "command");
            builder2.AddNamedParameter("NAME", "value");
            var sut2 = builder1.Build();

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void GetHashCode_CommandsAreSame_HashCodesAreSame()
        {
            // arrange
            var sut1 = CreateCommandInput(2, "command");
            var sut2 = CreateCommandInput(2, "command");

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashcode1, hashcode2);
        }

        [Fact]
        public void GetHashCode_NumberedParameterMissingFromOneCommand_HashCodesAreDifferent()
        {
            // arrange
            var builder1 = new CommandInput.Builder(2, "command");
            builder1.AddNumberedParameter("p1");
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(2, "command");
            var sut2 = builder2.Build();

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.NotEqual(hashcode1, hashcode2);
        }

        [Fact]
        public void GetHashCode_NumberedParametersAreSame_HashCodesAreSame()
        {
            // arrange
            var builder1 = new CommandInput.Builder(2, "command");
            builder1.AddNumberedParameter("p1");
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(2, "command");
            builder2.AddNumberedParameter("p1");
            var sut2 = builder2.Build();

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashcode1, hashcode2);
        }

        [Fact]
        public void GetHashCode_NamedParameterMissingFromOneCommand_HashCodesAreDifferent()
        {
            // arrange
            var builder1 = new CommandInput.Builder(2, "command");
            builder1.AddNamedParameter("name", "value");
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(2, "command");
            var sut2 = builder2.Build();

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.NotEqual(hashcode1, hashcode2);
        }

        [Fact]
        public void GetHashCode_NamedParametersAreSame_HashCodesAreSame()
        {
            // arrange
            var builder1 = new CommandInput.Builder(2, "command");
            builder1.AddNamedParameter("name", "value");
            var sut1 = builder1.Build();

            var builder2 = new CommandInput.Builder(2, "command");
            builder2.AddNamedParameter("name", "value");
            var sut2 = builder2.Build();

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashcode1, hashcode2);
        }

        [Theory]
        [InlineData(1, "command", 2, "command")]
        [InlineData(1, "command", 1, "other")]
        public void GetHashCode_CommandsAreDifferent_HashCodeAreDifferent(int sourceLine1, string commandName1, int sourceLine2, string commandName2)
        {
            // arrange
            var sut1 = CreateCommandInput(sourceLine1, commandName1);
            var sut2 = CreateCommandInput(sourceLine2, commandName2);

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.NotEqual(hashcode1, hashcode2);
        }

        [Fact]
        public void GetHashCode_CommandNamesCasingDifferent_HashCodeAreSame()
        {
            // arrange
            var sut1 = CreateCommandInput(1, "command");
            var sut2 = CreateCommandInput(1, "COMmAND");

            // act
            var hashcode1 = sut1.GetHashCode();
            var hashcode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashcode1, hashcode2);
        }

        private CommandInput CreateCommandInput(int sourceLine, string commandName)
        {
            var builder = new CommandInput.Builder(sourceLine, commandName);
            builder.AddNumberedParameter("p1");
            return builder.Build();
        }
    }
}
