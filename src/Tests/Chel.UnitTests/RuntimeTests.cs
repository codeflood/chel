using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Xunit;

namespace Chel.UnitTests
{
    public class RuntimeTests
    {
        [Fact]
        public void RegisterCommandType_TypeIsNull_ThrowsException()
        {
            // arrange
            var sut = new Runtime();
            Action sutAction = () => sut.RegisterCommandType(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("commandType", ex.ParamName);
        }

        [Fact]
        public void NewSession_WhenCalled_ReturnsSession()
        {
            // arrange
            var sut = new Runtime();

            // act
            var session = sut.NewSession();

            // assert
            Assert.NotNull(session);
        }

        [Fact]
        public void NewSession_WhenCalled_HelpCommandAvailable()
        {
            // arrange
            var sut = new Runtime();
            CommandResult commandResult = null;

            // act
            var session = sut.NewSession();
            session.Execute("help", result => commandResult = result);

            // assert
            Assert.IsType<ValueResult>(commandResult);
        }

        [Fact]
        public void RegisterCommandService_ServiceIsNull_ThrowsException()
        {
            // arrange
            var sut = new Runtime();
            Action sutAction = () => sut.RegisterCommandService<object>(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("service", ex.ParamName);
        }
    }
}