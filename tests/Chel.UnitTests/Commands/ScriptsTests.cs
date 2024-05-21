using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Commands;
using NSubstitute;
using Xunit;

namespace Chel.UnitTests.Commands;

public class ScriptsTests
{
    [Fact]
    public void Ctor_ScriptProviderIsNull_ThrowsException()
    {
        // arrange
        Action sutAction = () => new Scripts(null);

        // act, assert
        var ex = Assert.Throws<ArgumentNullException>(sutAction);
        Assert.Equal("scriptProvider", ex.ParamName);
    }

    [Fact]
    public void Execute_NoParametersSet_ListsAllScritps()
    {
        // arrange
        var scriptProvider = Substitute.For<IScriptProvider>();
        scriptProvider.GetScriptNames().Returns(new [] { new ExecutionTargetIdentifier("mod1", "script1") });

        var sut = new Scripts(scriptProvider);

        // act
        var result = sut.Execute() as ValueResult;

        // assert
        var commands = Assert.IsType<List>(result.Value);
        var name1 = Assert.IsType<Literal>(commands.Values[0]);
        Assert.Equal("mod1:script1", name1.Value);
    }
}