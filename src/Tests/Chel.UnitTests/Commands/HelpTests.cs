using System;
using Chel.Commands;
using Xunit;

namespace Chel.UnitTests.Commands
{
    public class HelpTests
    {
        [Fact]
        public void Ctor_CommandRegistryIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new Help(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandRegistry", ex.ParamName);
        }
    }
}