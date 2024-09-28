using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class ApplicationTextResolverTest
    {
        [Fact]
        public void Resolve_KeyIsNull_ReturnsEmpty()
        {
            var result = ApplicationTextResolver.Instance.Resolve(null!);
            Assert.Empty(result);
        }

        [Fact]
        public void Resolve_KeyIsEmpty_ReturnsEmpty()
        {
            var result = ApplicationTextResolver.Instance.Resolve("");
            Assert.Empty(result);
        }

        [Fact]
        public void Resolve_KnownKey_ReturnsText()
        {
            var result = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.ArgumentCannotBeNull);
            Assert.Equal("'{0}' cannot be null", result);
        }

        [Fact]
        public void Resolve_UnknownKey_ReturnsKey()
        {
            var result = ApplicationTextResolver.Instance.Resolve("INVALID_KEY");
            Assert.Equal("INVALID_KEY", result);
        }

        [Fact]
        public void ResolveAndFormat_KeyIsNull_ReturnsEmpty()
        {
            var result = ApplicationTextResolver.Instance.ResolveAndFormat(null!);
            Assert.Empty(result);
        }

        [Fact]
        public void ResolveAndFormat_KeyIsEmpty_ReturnsEmpty()
        {
            var result = ApplicationTextResolver.Instance.ResolveAndFormat("");
            Assert.Empty(result);
        }

        [Fact]
        public void ResolveAndFormat_KnownKey_ReturnsTextFormatted()
        {
            var result = ApplicationTextResolver.Instance.ResolveAndFormat(ApplicationTexts.ArgumentCannotBeNull, "foo");
            Assert.Equal("'foo' cannot be null", result);
        }

        [Fact]
        public void ResolveAndFormat_UnknownKey_ReturnsKey()
        {
            var result = ApplicationTextResolver.Instance.ResolveAndFormat("INVALID_KEY", "foo");
            Assert.Equal("INVALID_KEY", result);
        }
    }
}
