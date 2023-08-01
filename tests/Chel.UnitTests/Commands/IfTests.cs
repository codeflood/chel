using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Commands;
using NSubstitute;
using Xunit;

namespace Chel.UnitTests.Commands
{
    public class IfTests
    {
        [Fact]
        public void Ctor_SessionIsNull_ThrowsException()
        {
            // arrange
            Action sutAction = () => new If(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("session", ex.ParamName);
        }

        [Fact]
        public void Execute_ConditionIsTrue_ExecutesScript()
        {
            // arrange
            var session = Substitute.For<ISession>();
            var sut = new If(session)
            {
                ShouldExecute = true,
                ScriptBlock = "a"
            };

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<SuccessResult>(result);
            session.Received().Execute("a");
        }

        [Fact]
        public void Execute_ConditionIsTrueScriptIsNull_DoesNotExecuteScript()
        {
            // arrange
            var session = Substitute.For<ISession>();
            var sut = new If(session)
            {
                ShouldExecute = true,
                ScriptBlock = null
            };

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<SuccessResult>(result);
            session.DidNotReceive().Execute(Arg.Any<string>());
        }

        [Fact]
        public void Execute_ConditionIsTrueScriptIsNull_DoesNotExecuteElseScript()
        {
            // arrange
            var session = Substitute.For<ISession>();
            var sut = new If(session)
            {
                ShouldExecute = true,
                ScriptBlock = null,
                ElseScriptBlock = "b"
            };

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<SuccessResult>(result);
            session.DidNotReceive().Execute(Arg.Any<string>());
        }

        [Fact]
        public void Execute_ConditionIsTrue_DoesNotExecuteElseScript()
        {
            // arrange
            var session = Substitute.For<ISession>();
            var sut = new If(session)
            {
                ShouldExecute = true,
                ScriptBlock = "a",
                ElseScriptBlock = "b"
            };

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<SuccessResult>(result);
            session.DidNotReceive().Execute("b");
        }

        [Fact]
        public void Execute_ConditionIsFalse_DoesNotExecuteScript()
        {
            // arrange
            var session = Substitute.For<ISession>();
            var sut = new If(session)
            {
                ShouldExecute = false,
                ScriptBlock = "a"
            };

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<SuccessResult>(result);
            session.DidNotReceive().Execute("a");
        }

        [Fact]
        public void Execute_ConditionIsFalse_ExecutesElseScript()
        {
            // arrange
            var session = Substitute.For<ISession>();
            var sut = new If(session)
            {
                ShouldExecute = false,
                ScriptBlock = "a",
                ElseScriptBlock = "b"
            };

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<SuccessResult>(result);
            session.Received().Execute("b");
        }

        [Fact]
        public void Execute_ConditionIsFalseElseScriptIsNull_DoesNotExecuteScript()
        {
            // arrange
            var session = Substitute.For<ISession>();
            var sut = new If(session)
            {
                ShouldExecute = false,
                ScriptBlock = "a",
                ElseScriptBlock = null
            };

            // act
            var result = sut.Execute();

            // assert
            Assert.IsType<SuccessResult>(result);
            session.DidNotReceive().Execute(Arg.Any<string>());
        }
    }
}