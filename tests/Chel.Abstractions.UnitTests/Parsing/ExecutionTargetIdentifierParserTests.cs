using Chel.Abstractions.Parsing;
using Xunit;

namespace Chel.Abstractions.UnitTests.Parsing
{
    public class ExecutionTargetIdentifierParserTests
    {
        private readonly ExecutionTargetIdentifierParser _sut;

        public ExecutionTargetIdentifierParserTests()
        {
            _sut = new ExecutionTargetIdentifierParser();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\r")]
        public void Parse_InputIsInvalid_ReturnsEmptyCommandResult(string input)
        {
            var result = _sut.Parse(input);
            Assert.Null(result.Module);
            Assert.Empty(result.Name);
        }

        [Fact]
        public void Parse_InputIsCommandOnly_ReturnsCommandResult()
        {
            var result = _sut.Parse("cmd");
            Assert.Null(result.Module);
            Assert.Equal("cmd", result.Name);
        }

        [Fact]
        public void Parse_InputIsCommandAndModule_ReturnsCommandAndModuleResult()
        {
            var result = _sut.Parse("mod:cmd");
            Assert.Equal("mod", result.Module);
            Assert.Equal("cmd", result.Name);
        }

        [Fact]
        public void Parse_InputIsModuleOnly_ReturnsCommandAndModuleResult()
        {
            var result = _sut.Parse("mod:");
            Assert.Equal("mod", result.Module);
            Assert.Empty(result.Name);
        }
    }
}
