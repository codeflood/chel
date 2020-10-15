using System;
using Xunit;

namespace Chel.UnitTests
{
    public class PhraseDictionaryTests
    {
        [Fact]
        public void GetPhrase_PhraseKeyIsNull_ThrowsException()
        {
            // arrange
            var sut = new PhraseDictionary();
            Action sutAction = () => sut.GetPhrase(null, "en");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("phraseKey", ex.ParamName);
        }

        [Theory]
        [InlineData(Texts.PhraseKeys.AvailableCommands, "Available commands")]
        [InlineData(Texts.PhraseKeys.Required, "Required")]
        [InlineData(Texts.PhraseKeys.Usage, "usage")]
        [InlineData(Texts.PhraseKeys.NoVariablesSet, "No variables set.")]
        [InlineData(Texts.PhraseKeys.VariableNotSet, "Variable '{0}' is not set.")]
        public void GetPhrase_PhraseKeyIsValid_ReturnsPhrase(string key, string expected)
        {
            // arrange
            var sut = new PhraseDictionary();

            // act
            var result = sut.GetPhrase(key, "en");

            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPhrase_PhraseNotPresent_ReturnsPhraseKey()
        {
            // arrange
            var sut = new PhraseDictionary();

            // act
            var result = sut.GetPhrase("invalid phrase", "en");

            // assert
            Assert.Equal("invalid phrase", result);
        }
    }
}