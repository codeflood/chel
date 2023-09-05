using System;
using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class ExceptionFactoryTest
    {
        [Fact]
        public void CreateArgumentException_TextKeyIsNull_Throws()
        {
            Action sutAction = () => ExceptionFactory.CreateArgumentException(null, "param");
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("textKey", ex.ParamName);
        }

        [Fact]
        public void CreateArgumentException_TextKeyIsEmpty_Throws()
        {
            Action sutAction = () => ExceptionFactory.CreateArgumentException("", "param");
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("textKey", ex.ParamName);
        }

        [Fact]
        public void CreateArgumentException_ParamNameIsNull_Throws()
        {
            Action sutAction = () => ExceptionFactory.CreateArgumentException("PHRASE", null);
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("paramName", ex.ParamName);
        }

        [Fact]
        public void CreateArgumentException_ParamNameIsEmpty_Throws()
        {
            Action sutAction = () => ExceptionFactory.CreateArgumentException("PHRASE", "");
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("paramName", ex.ParamName);
        }

        [Fact]
        public void CreateArgumentException_WhenCalled_ReturnsException()
        {
            var result = ExceptionFactory.CreateArgumentException("PHRASE", "arg");
            Assert.StartsWith("PHRASE", result.Message);
            Assert.Equal("arg", result.ParamName);
        }

        [Fact]
        public void CreateArgumentException_WhenCalledWithArgs_FormatsMessageWithArgs()
        {
            var result = ExceptionFactory.CreateArgumentException("_{0}_", "arg", "arg1");
            Assert.StartsWith("_arg1_", result.Message);
            Assert.Equal("arg", result.ParamName);
        }

        [Fact]
        public void CreateInvalidOperationException_TextKeyIsNull_Throws()
        {
            Action sutAction = () => ExceptionFactory.CreateInvalidOperationException(null);
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("textKey", ex.ParamName);
        }

        [Fact]
        public void CreateInvalidOperationException_TextKeyIsEmpty_Throws()
        {
            Action sutAction = () => ExceptionFactory.CreateInvalidOperationException("");
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("textKey", ex.ParamName);
        }

        [Fact]
        public void CreateInvalidOperationException_WhenCalled_ReturnsException()
        {
            var result = ExceptionFactory.CreateInvalidOperationException("PHRASE");
            Assert.Equal("PHRASE", result.Message);
        }

        [Fact]
        public void CreateInvalidOperationException_WhenCalledWithArgs_FormatsMessageWithArgs()
        {
            var result = ExceptionFactory.CreateInvalidOperationException("_{0}_", "arg1");
            Assert.Equal("_arg1_", result.Message);
        }
    }
}
