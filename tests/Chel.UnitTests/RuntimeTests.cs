using System;
using System.Collections.Generic;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Commands;
using Xunit;
using Xunit.Abstractions;

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
            var session = sut.NewSession(_ => {});

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
            var session = sut.NewSession(result => commandResult = result);
            session.Execute("help");

            // assert
            Assert.IsType<ValueResult>(commandResult);
        }

        [Fact]
        public void NewSession_WhenCalled_VarCommandAvailable()
        {
            // arrange
            var sut = new Runtime();
            CommandResult commandResult = null;

            // act
            var session = sut.NewSession(result => commandResult = result);
            session.Execute("var");

            // assert
            Assert.IsType<ValueResult>(commandResult);
        }

        [Fact]
        public void NewSession_IfCommandRegistered_IfCommandAvailable()
        {
            // arrange
            var sut = new Runtime();
            sut.RegisterCommandType(typeof(If));
            sut.RegisterCommandType(typeof(Echo));
            List<CommandResult> commandResults = new();

            // act
            var session = sut.NewSession(result => commandResults.Add(result));
            session.Execute("if true (echo hi)");

            // assert
            Assert.IsType<SuccessResult>(commandResults[1]);

            var valueResult = Assert.IsType<ValueResult>(commandResults[0]);
            var literalResult = Assert.IsType<Literal>(valueResult.Value);
            Assert.Equal("hi", literalResult.Value);
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