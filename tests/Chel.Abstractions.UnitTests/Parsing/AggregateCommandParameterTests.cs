using System.Collections.Generic;
using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class AggregateCommandParameterTests
    {
        [Fact]
        public void Ctor_ParametersIsNull_SetsPropertyToEmptyCollection()
        {
            // arrange, act
            var sut = new AggregateCommandParameter(null);

            // assert
            Assert.Empty(sut.Parameters);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsProperties()
        {
            // arrange
            var param1 = new LiteralCommandParameter("lit");
            var param2 = new VariableCommandParameter("var");

            // act
            var sut = new AggregateCommandParameter(new List<CommandParameter> { param1, param2 });

            // assert
            Assert.Equal(new CommandParameter[] { param1, param2 }, sut.Parameters);
        }

        [Fact]
        public void Equals_ValuesAreBothEmpty_ReturnsTrue()
        {
            // arrange
            var sut1 = new AggregateCommandParameter(null);
            var sut2 = new AggregateCommandParameter(null);

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ValuesBothContainEqualElements_ReturnsTrue()
        {
            // arrange
            var param11 = new LiteralCommandParameter("lit");
            var param21 = new LiteralCommandParameter("lit");

            var param12 = new VariableCommandParameter("var");
            var param22 = new VariableCommandParameter("var");

            var sut1 = new AggregateCommandParameter(new CommandParameter[] { param11, param12 });
            var sut2 = new AggregateCommandParameter(new CommandParameter[] { param21, param22 });

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_SameValuesOutOfOrder_ReturnsFalse()
        {
            // arrange
            var param11 = new LiteralCommandParameter("lit");
            var param21 = new LiteralCommandParameter("lit");

            var param12 = new VariableCommandParameter("var");
            var param22 = new VariableCommandParameter("var");

            var sut1 = new AggregateCommandParameter(new CommandParameter[] { param11, param12 });
            var sut2 = new AggregateCommandParameter(new CommandParameter[] { param22, param21 });

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            // arrange
            var param1 = new LiteralCommandParameter("lit");
            var param2 = new LiteralCommandParameter("other");

            var sut1 = new AggregateCommandParameter(new CommandParameter[] { param1 });
            var sut2 = new AggregateCommandParameter(new CommandParameter[] { param2 });

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ValuesAreEqual_ReturnsSameHashCode()
        {
            // arrange
            var param11 = new LiteralCommandParameter("lit");
            var param21 = new LiteralCommandParameter("lit");

            var param12 = new VariableCommandParameter("var");
            var param22 = new VariableCommandParameter("var");

            var sut1 = new AggregateCommandParameter(new CommandParameter[] { param11, param12 });
            var sut2 = new AggregateCommandParameter(new CommandParameter[] { param21, param22 });

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_ValuesAreEqual_ReturnsDifferentHashCodes()
        {
            // arrange
            var param11 = new LiteralCommandParameter("lit");
            var param21 = new LiteralCommandParameter("lit");

            var param12 = new VariableCommandParameter("var");
            var param22 = new VariableCommandParameter("var2");

            var sut1 = new AggregateCommandParameter(new CommandParameter[] { param11, param12 });
            var sut2 = new AggregateCommandParameter(new CommandParameter[] { param21, param22 });

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.NotEqual(hashCode1, hashCode2);
        }
    }
}