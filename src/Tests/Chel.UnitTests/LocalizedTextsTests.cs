using System;
using System.Globalization;
using Xunit;

namespace Chel.UnitTests
{
    public class LocalisedTextsTests
    {
        [Fact]
        public void AddText_TextIsNull_ThrowsException()
        {
            // arrange
            var sut = new LocalisedTexts();
            Action sutAction = () => sut.AddText(null, "en-AU");

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("text", ex.ParamName);
        }

        [Fact]
        public void AddText_TextIsEmpty_ThrowsException()
        {
            // arrange
            var sut = new LocalisedTexts();
            Action sutAction = () => sut.AddText("", "en-AU");

            // act, assert
            var ex = Assert.Throws<ArgumentException>(sutAction);
            Assert.Equal("text", ex.ParamName);
            Assert.Contains("Parameter 'text' cannot be empty", ex.Message);
        }

        [Fact]
        public void AddText_CultureNameIsEmpty_TextSetForInvariantCulture()
        {
            // arrange
            var sut = new LocalisedTexts();

            // act
            sut.AddText("text", "");

            // assert
            var result = sut.GetText(CultureInfo.InvariantCulture.Name);
            Assert.Equal("text", result);
        }

        [Fact]
        public void AddText_CultureAlreadyAdded_ThrowsException()
        {
            // arrange
            var sut = new LocalisedTexts();
            sut.AddText("text", "en");
            Action sutAction = () => sut.AddText("text", "en");

            // act, assert
            var ex = Assert.Throws<InvalidOperationException>(sutAction);
            Assert.Contains("Text for culture 'en' has already been added", ex.Message);
        }

        [Fact]
        public void GetText_CultureNameIsNull_ThrowsException()
        {
            // arrange
            var sut = new LocalisedTexts();
            sut.AddText("text", "en");
            Action sutAction = () => sut.GetText(null);

            // act, assert
            var ex = Assert.Throws<ArgumentNullException>(sutAction);
            Assert.Equal("cultureName", ex.ParamName);
        }

        [Fact]
        public void GetText_TextIsPresent_ReturnsText()
        {
            // arrange
            var sut = new LocalisedTexts();
            sut.AddText("text", "en");

            // act
            var result = sut.GetText("en");

            // assert
            Assert.Equal("text", result);
        }

        [Fact]
        public void GetText_TextNotPresent_ReturnsNull()
        {
            // arrange
            var sut = new LocalisedTexts();
            sut.AddText("text", "en");

            // act
            var result = sut.GetText("fr");

            // assert
            Assert.Null(result);
        }

        [Fact]
        public void GetText_LessSpecificTextPresent_ReturnsLessSpecificText()
        {
            // arrange
            var sut = new LocalisedTexts();
            sut.AddText("text", "en");

            // act
            var result = sut.GetText("en-AU");

            // assert
            Assert.Equal("text", result);
        }

        [Theory]
        [InlineData("fr")]
        [InlineData("en-AU")]
        public void GetText_OnlyInvariantTextPresent_ReturnsInvariantText(string cultureName)
        {
            // arrange
            var sut = new LocalisedTexts();
            sut.AddText("text", null);

            // act
            var result = sut.GetText(cultureName);

            // assert
            Assert.Equal("text", result);
        }
    }
}