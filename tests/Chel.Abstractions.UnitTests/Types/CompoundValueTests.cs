using System;
using System.Collections.Generic;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
using Xunit;

namespace Chel.Abstractions.UnitTests.Types
{
    public class CompoundValueTests
    {
        [Fact]
        public void Ctor_NoValues_SetsPropertyToEmptyCollection()
        {
            // arrange, act
            var sut = new CompoundValue(new ICommandParameter[0]);

            // assert
            Assert.Empty(sut.Values);
        }

        [Fact]
        public void Ctor_LiteralProvided_SetsProperty()
        {
            // arrange
            var value = new Literal("lit");

            // act
            var sut = new CompoundValue(new[]{value});

            // assert
            Assert.Equal(new[] { value }, sut.Values);
        }

        [Fact]
        public void Ctor_VariableReferenceProvided_SetsProperty()
        {
            // arrange
            var value = new VariableReference("var");

            // act
            var sut = new CompoundValue(new[]{value});

            // assert
            Assert.Equal(new[] { value }, sut.Values);
        }

        [Theory]
        [MemberData(nameof(Ctor_ListContainsUnexpectedTypes_ThrowsException_DataSource))]
        public void Ctor_ListContainsUnexpectedTypes_ThrowsException(IReadOnlyList<ICommandParameter> input)
        {
            // arrange
            Action sutAction = () => new CompoundValue(input);

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("values", ex.ParamName);
        }

        public static IEnumerable<object[]> Ctor_ListContainsUnexpectedTypes_ThrowsException_DataSource()
        {
            yield return new[] {
                new ICommandParameter[] {
                    new Literal("val"),
                    new List(new[] { new Literal("val") })
                }
            };

            yield return new[] {
                new ICommandParameter[] {
                    new CompoundValue(new[]{new Literal("val")})
                }
            };
        }

        [Fact]
        public void Equals_ValuesBothContainEqualElements_ReturnsTrue()
        {
            // arrange
            var param11 = new Literal("val1");
            var param21 = new Literal("val1");

            var param12 = new VariableReference("val2");
            var param22 = new VariableReference("val2");

            var sut1 = new CompoundValue(new ICommandParameter[] { param11, param12 });
            var sut2 = new CompoundValue(new ICommandParameter[] { param21, param22 });

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_SameValuesOutOfOrder_ReturnsFalse()
        {
            // arrange
            var param11 = new Literal("val1");
            var param21 = new Literal("val1");

            var param12 = new VariableReference("val2");
            var param22 = new VariableReference("val2");

            var sut1 = new CompoundValue(new ICommandParameter[] { param11, param12 });
            var sut2 = new CompoundValue(new ICommandParameter[] { param22, param21 });

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            // arrange
            var param1 = new Literal("val");
            var param2 = new Literal("other");

            var sut1 = new CompoundValue(new[]{param1});
            var sut2 = new CompoundValue(new[]{param2});

            // act
            var result = sut1.Equals(sut2);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_ValuesAreEqual_ReturnsSameHashCode()
        {
            // arrange
            var param11 = new Literal("val1");
            var param21 = new Literal("val1");

            var param12 = new Literal("val2");
            var param22 = new Literal("val2");

            var sut1 = new CompoundValue(new[] { param11, param12 });
            var sut2 = new CompoundValue(new[] { param21, param22 });

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_ValuesAreDifferent_ReturnsDifferentHashCodes()
        {
            // arrange
            var param11 = new Literal("val1");
            var param21 = new Literal("val1");

            var param12 = new Literal("val2");
            var param22 = new Literal("val3");

            var sut1 = new CompoundValue(new[] { param11, param12 });
            var sut2 = new CompoundValue(new[] { param21, param22 });

            // act
            var hashCode1 = sut1.GetHashCode();
            var hashCode2 = sut2.GetHashCode();

            // assert
            Assert.NotEqual(hashCode1, hashCode2);
        }

        [Fact]
        public void ToString_SingleValue_ReturnsValue()
        {
            // arrange
            var sut = new CompoundValue(new[]{new Literal("val")});

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("val", result);
        }

        [Fact]
        public void ToString_MultipleValues_ReturnsValues()
        {
            // arrange
            var sut = new CompoundValue(new [] { new Literal("val"), new Literal("val2") });

            // act
            var result = sut.ToString();

            // assert
            Assert.Equal("valval2", result);
        }
    }
}