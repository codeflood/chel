using System;
using Chel.Abstractions;
using Xunit;

namespace Chel.UnitTests;

public class DirectoryScriptProviderTests
{
    [Fact]
    public void Ctor_PathIsNull_Throws()
    {
        // arrange
        Action sutAction = () => new DirectoryScriptProvider(null!);

        // act
        var ex = Assert.Throws<ArgumentNullException>(sutAction);

        // assert
        Assert.Equal("path", ex.ParamName);
    }

    [Fact]
    public void Ctor_PathIsEmpty_DoesNotThrow()
    {
        // arrange, act
        var sut = new DirectoryScriptProvider("");

        // assert
        Assert.NotNull(sut);
    }

    [Fact]
    public void GetScriptNames_InvalidPath_ReturnsEmpty()
    {
        // arrange
        var sut = new DirectoryScriptProvider("invalid");

        // act
        var names = sut.GetScriptNames();

        // assert
        Assert.Empty(names);
    }

    [Fact]
    public void GetScriptNames_ValidPath_ReturnsScriptFiles()
    {
        // arrange
        var sut = new DirectoryScriptProvider("scripts");

        // act
        var names = sut.GetScriptNames();

        // assert
        Assert.Collection(names,
            x => x.Equals(new ExecutionTargetIdentifier(null, "test1")),
            x => x.Equals(new ExecutionTargetIdentifier(null, "test2")),
            x => x.Equals(new ExecutionTargetIdentifier("mod1", "test1")),
            x => x.Equals(new ExecutionTargetIdentifier("mod1", "test2"))
        );
    }

    [Fact]
    public void GetScriptSource_InvalidPath_ReturnsNull()
    {
        // arrange
        var sut = new DirectoryScriptProvider("invalid");

        // act
        var names = sut.GetScriptSource(null, "invalid");

        // assert
        Assert.Null(names);
    }

    [Fact]
    public void GetScriptSource_InvalidScriptName_ReturnsNull()
    {
        // arrange
        var sut = new DirectoryScriptProvider("scripts");

        // act
        var result = sut.GetScriptSource(null, "invalid");

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void GetScriptSource_InvalidModuleName_ReturnsNull()
    {
        // arrange
        var sut = new DirectoryScriptProvider("scripts");

        // act
        var result = sut.GetScriptSource("invalid", "test1");

        // assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("test1")]
    [InlineData("Test1")]
    [InlineData("TEST1")]
    public void GetScriptSource_ValidScriptName_ReturnsSource(string scriptName)
    {
        // arrange
        var sut = new DirectoryScriptProvider("scripts");

        // act
        var result = sut.GetScriptSource(null, scriptName);

        // assert
        Assert.Equal("echo test11\necho test21\n", result);
    }

    [Fact]
    public void GetScriptSource_ModuleIsParentDirectory_ReturnsNull()
    {
        // arrange
        var sut = new DirectoryScriptProvider("scripts");

        // act
        var result = sut.GetScriptSource("..", "test-script");

        // assert
        Assert.Null(result);
    }
}
