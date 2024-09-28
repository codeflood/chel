using System;
using Chel.Abstractions.Types;
using Chel.Abstractions.UnitTests.SampleCommands;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class PropertyDescriptorTests
    {
        [Fact]
        public void Ctor_PropertyInfoIsNull_Throws()
        {
            // arrange
            Action sutAction = () => new PropertyDescriptor(null!);

            // act
            var ex = Assert.Throws<ArgumentNullException>(sutAction);

            // assert
            Assert.Equal("property", ex.ParamName);
        }

        [Theory]
        [InlineData(typeof(SampleCommand), nameof(SampleCommand.Parameter))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.Dictionary))]
        public void IsTypeListCompatible_NonListType_ReturnsFalse(Type commandType, string propertyName)
        {
            // arrange
            var property = commandType.GetProperty(propertyName);

            // act
            var sut = new PropertyDescriptor(property!);

            // assert
            Assert.False(sut.IsTypeListCompatible);
        }

        [Theory]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.Array))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.Enumerable))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.List))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.IntList))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.Collection))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.ReadOnlyCollection))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.ConcreteList))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.MapList))]
        public void IsTypeListCompatible_ListType_ReturnsTrue(Type commandType, string propertyName)
        {
            // arrange
            var property = commandType.GetProperty(propertyName);

            // act
            var sut = new PropertyDescriptor(property!);

            // assert
            Assert.True(sut.IsTypeListCompatible);
        }

        [Theory]
        [InlineData(typeof(SampleCommand), nameof(SampleCommand.Parameter))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.List))]
        public void IsTypeMapCompatible_NonMapType_ReturnsFalse(Type commandType, string propertyName)
        {
            // arrange
            var property = commandType.GetProperty(propertyName);

            // act
            var sut = new PropertyDescriptor(property!);

            // assert
            Assert.False(sut.IsTypeMapCompatible);
        }

        [Theory]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.Dictionary))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.AbstractDictionary))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.InvalidKeyTypeParam))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.IntDictionary))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.ListDictionary))]
        public void IsTypeMapCompatible_MapType_ReturnsTrue(Type commandType, string propertyName)
        {
            // arrange
            var property = commandType.GetProperty(propertyName);

            // act
            var sut = new PropertyDescriptor(property!);

            // assert
            Assert.True(sut.IsTypeMapCompatible);
        }

        [Theory]
        [InlineData(typeof(SampleCommand), nameof(SampleCommand.Parameter))]
        public void GenericValueType_NonGenericType_ReturnsNull(Type commandType, string propertyName)
        {
            // arrange
            var property = commandType.GetProperty(propertyName);

            // act
            var sut = new PropertyDescriptor(property!);

            // assert
            Assert.Null(sut.GenericValueType);
        }

        [Theory]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.Array), typeof(string))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.Enumerable), typeof(string))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.List), typeof(string))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.IntList), typeof(int))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.Collection), typeof(string))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.ReadOnlyCollection), typeof(string))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.ConcreteList), typeof(string))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.MapList), typeof(Map))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.Dictionary), typeof(string))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.AbstractDictionary), typeof(string))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.InvalidKeyTypeParam), typeof(string))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.IntDictionary), typeof(int))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.ListDictionary), typeof(List))]
        public void GenericValueType_PropertyTypeIsGeneric_ReturnsExpectedType(Type commandType, string propertyName, Type expectedType)
        {
            // arrange
            var property = commandType.GetProperty(propertyName);

            // act
            var sut = new PropertyDescriptor(property!);

            // assert
            Assert.Equal(expectedType, sut.GenericValueType);
        }

        [Theory]
        [InlineData(typeof(SampleCommand), nameof(SampleCommand.Parameter))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.Array))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.Enumerable))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.List))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.IntList))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.Collection))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.ReadOnlyCollection))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.ConcreteList))]
        [InlineData(typeof(ListParameterCommand), nameof(ListParameterCommand.MapList))]
        public void GenericKeyType_NonMapType_ReturnsNull(Type commandType, string propertyName)
        {
            // arrange
            var property = commandType.GetProperty(propertyName);

            // act
            var sut = new PropertyDescriptor(property!);

            // assert
            Assert.Null(sut.GenericKeyType);
        }

        [Theory]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.Dictionary), typeof(string))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.AbstractDictionary), typeof(string))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.InvalidKeyTypeParam), typeof(int))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.IntDictionary), typeof(string))]
        [InlineData(typeof(MapParameterCommand), nameof(MapParameterCommand.ListDictionary), typeof(string))]
        public void GenericKeyType_MapType_ReturnsExpectedType(Type commandType, string propertyName, Type expectedType)
        {
            // arrange
            var property = commandType.GetProperty(propertyName);

            // act
            var sut = new PropertyDescriptor(property!);

            // assert
            Assert.Equal(expectedType, sut.GenericKeyType);
        }
    }
}