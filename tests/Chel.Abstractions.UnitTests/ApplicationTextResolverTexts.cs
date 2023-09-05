using Xunit;

namespace Chel.Abstractions.UnitTests
{
    public class ApplicationTextResolverTest
    {
        [Fact]
        public void Resolve_KeyIsNull_ReturnsEmpty()
        {
            var result = ApplicationTextResolver.Instance.Resolve(null, "en");
            Assert.Empty(result);
        }

        [Fact]
        public void Resolve_KeyIsEmpty_ReturnsEmpty()
        {
            var result = ApplicationTextResolver.Instance.Resolve("", "en");
            Assert.Empty(result);
        }

        [Fact]
        public void Resolve_CultureIsNull_ReturnsDefault()
        {
            var result = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.ArgumentCannotBeNull, null);
            Assert.Equal("'{0}' cannot be null", result);
        }

        [Fact]
        public void Resolve_CultureIsEmpty_ReturnsDefault()
        {
            var result = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.ArgumentCannotBeNull, "");
            Assert.Equal("'{0}' cannot be null", result);
        }

        [Fact]
        public void Resolve_UnknownKey_ReturnsKey()
        {
            var result = ApplicationTextResolver.Instance.Resolve("INVALID_KEY", "");
            Assert.Equal("INVALID_KEY", result);
        }
    }
}
