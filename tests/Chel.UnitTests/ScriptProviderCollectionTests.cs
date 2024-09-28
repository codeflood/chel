using System.Collections.Generic;
using Chel.Abstractions;
using NSubstitute;
using Xunit;

namespace Chel.UnitTests;

public class ScriptProviderCollectionTests
{
    [Fact]
    public void Ctor_ProvidersIsNull_DoesNotThrow()
    {
        new ScriptProviderCollection(null!);
    }

    [Fact]
    public void Ctor_ProvidersIsEmpty_DoesNotThrow()
    {
        new ScriptProviderCollection(new IScriptProvider[0]);
    }

    [Fact]
    public void GetScriptNames_NoProviders_ReturnsEmpty()
    {
        // arrange
        var sut = new ScriptProviderCollection(null!);

        // act
        var result = sut.GetScriptNames();

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetScriptNames_WhenCalled_ReturnsAllNames()
    {
        // arrange
        var script1 = new ExecutionTargetIdentifier(null, "script1");
        var script2 = new ExecutionTargetIdentifier(null, "script2");
        var script3 = new ExecutionTargetIdentifier(null, "script3");
        var script4 = new ExecutionTargetIdentifier(null, "script4");

        var provider1 = Substitute.For<IScriptProvider>();
        provider1.GetScriptNames().Returns(
            new List<ExecutionTargetIdentifier> { script1, script2 }
        );

        var provider2 = Substitute.For<IScriptProvider>();
        provider2.GetScriptNames().Returns(
            new List<ExecutionTargetIdentifier> { script3, script4 }
        );

        var sut = new ScriptProviderCollection(new[] { provider1, provider2 });

        // act
        var result = sut.GetScriptNames();

        // assert
        Assert.Collection(result,
            x => x.Equals(script1),
            x => x.Equals(script2),
            x => x.Equals(script3),
            x => x.Equals(script4)
        );
    }

    [Fact]
    public void GetScriptNames_SameTargetInMultipleProviders_ReturnsUniqueTargets()
    {
        // arrange
        var script1_1 = new ExecutionTargetIdentifier("mod1", "script1");
        var script2_1 = new ExecutionTargetIdentifier("mod2", "script1");
        var script2_1_dup = new ExecutionTargetIdentifier("mod2", "script1");

        var provider1 = Substitute.For<IScriptProvider>();
        provider1.GetScriptNames().Returns(
            new List<ExecutionTargetIdentifier> { script2_1 }
        );

        var provider2 = Substitute.For<IScriptProvider>();
        provider1.GetScriptNames().Returns(
            new List<ExecutionTargetIdentifier> { script1_1, script2_1_dup }
        );

        var sut = new ScriptProviderCollection(new[] { provider1, provider2 });

        // act
        var result = sut.GetScriptNames();

        // assert
        Assert.Collection(result,
            x => x.Equals(script1_1),
            x => x.Equals(script2_1)
        );
    }

    [Fact]
    public void GetScriptSource_ScriptNotPresent_ReturnsNull()
    {
        // arrange
        var provider1 = Substitute.For<IScriptProvider>();
        var provider2 = Substitute.For<IScriptProvider>();

        provider1.GetScriptSource(Arg.Any<string>(), Arg.Any<string>()).Returns((string)null!);
        provider2.GetScriptSource(Arg.Any<string>(), Arg.Any<string>()).Returns((string)null!);

        var sut = new ScriptProviderCollection(new[] { provider1, provider2 });

        // act
        var result = sut.GetScriptSource(null, "script1");

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void GetScriptSource_WhenCalled_ReturnsSourceFromProvider()
    {
        // arrange
        var provider1 = Substitute.For<IScriptProvider>();
        var provider2 = Substitute.For<IScriptProvider>();

        var script = "echo hello";
        provider2.GetScriptSource("mod1", "script1").Returns(script);

        var sut = new ScriptProviderCollection(new[] { provider1, provider2 });

        // act
        var result = sut.GetScriptSource("mod1", "script1");

        // assert
        Assert.Equal(script, result);
    }

    [Fact]
    public void GetScriptSource_MultipleScriptsSameName_ReturnsLastScriptSource()
    {
        // arrange
        var provider1 = Substitute.For<IScriptProvider>();
        var script1 = "echo hello";
        provider1.GetScriptSource("mod1", "script1").Returns(script1);

        var provider2 = Substitute.For<IScriptProvider>();
        var script2 = "echo (hello 2)";
        provider2.GetScriptSource("mod1", "script1").Returns(script2);

        var sut = new ScriptProviderCollection(new[] { provider1, provider2 });

        // act
        var result = sut.GetScriptSource("mod1", "script1");

        // assert
        Assert.Equal(script2, result);
    }
}
