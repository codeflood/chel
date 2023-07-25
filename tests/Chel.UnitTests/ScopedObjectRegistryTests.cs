using System;
using Chel.UnitTests.SampleObjects;
using Xunit;

namespace Chel.UnitTests
{
    public class ScopedObjectRegistryTests
    {
        [Fact]
        public void Register_TypeMissingParameterlessConstructor_ThrowsException()
        {
            // arrange
            var sut = new ScopedObjectRegistry();
            Action sutAction = () => sut.Register<NoParameterlessConstructorObject>();

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Type Chel.UnitTests.SampleObjects.NoParameterlessConstructorObject requires a parameterless constructor", ex.Message);
        }

        [Fact]
        public void RegisterInstance_InstanceIsNull_ThrowsException()
        {
            // arrange
            var sut = new ScopedObjectRegistry();
            Action sutAction = () => sut.RegisterInstance<GoodObject>(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("instance", ex.ParamName);
        }

        [Fact]
        public void ResolveRegisteredInstance_InstanceRegistered_ReturnsInstance()
        {
            // arrange
            var sut = new ScopedObjectRegistry();
            var instance = new GoodObject();

            // act
            sut.RegisterInstance(instance);
            var result = sut.Resolve(typeof(GoodObject));

            // assert
            Assert.Same(instance, result);
        }

        [Fact]
        public void ResolveRegisteredInstance_TypeAlreadyRegistered_ReturnsInstance()
        {
            // arrange
            var sut = new ScopedObjectRegistry();
            sut.Register<GoodObject>();
            var instance = new GoodObject();

            // act
            sut.RegisterInstance(instance);
            var result = sut.Resolve(typeof(GoodObject));

            // assert
            Assert.Same(instance, result);
        }

        [Fact]
        public void Resolve_TypeNotRegistered_ReturnsNull()
        {
            // arrange
            var sut = new ScopedObjectRegistry();
            
            // act
            var result = sut.Resolve(typeof(GoodObject));

            // assert
            Assert.Null(result);
        }

        [Fact]
        public void Resolve_RegisteredType_ReturnsInstance()
        {
            // arrange
            var sut = new ScopedObjectRegistry();
            sut.Register<GoodObject>();

            // act
            var result = sut.Resolve(typeof(GoodObject));

            // assert
            Assert.IsType<GoodObject>(result);
        }

        [Fact]
        public void Resolve_RegisteredType_ReturnsSameInstance()
        {
            // arrange
            var sut = new ScopedObjectRegistry();
            sut.Register<GoodObject>();

            // act
            var result1 = sut.Resolve(typeof(GoodObject));
            var result2 = sut.Resolve(typeof(GoodObject));

            // assert
            Assert.Same(result1, result2);
        }

        [Fact]
        public void CreateScope_WhenCalled_IncludesRegisteredTypes()
        {
            // arrange
            var sut = new ScopedObjectRegistry();
            sut.Register<GoodObject>();

            // act
            var scope = sut.CreateScope();

            // assert
            var result = scope.Resolve(typeof(GoodObject));
            Assert.IsType<GoodObject>(result);
        }

        [Fact]
        public void CreateScope_WhenCalled_ExcludesInstances()
        {
            // arrange
            var sut = new ScopedObjectRegistry();
            sut.Register<GoodObject>();
            var result1 = sut.Resolve(typeof(GoodObject));

            // act
            var scope = sut.CreateScope();

            // assert
            var result2 = scope.Resolve(typeof(GoodObject));
            Assert.NotSame(result1, result2);
        }
    }
}