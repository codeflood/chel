namespace Chel.Abstractions
{
    /// <summary>
    /// A localizable dictionary of phrases.
    /// </summary>
    public interface IPhraseDictionary
    {
        /// <summary>
        /// Gets a phrase in the specified culture.
        /// </summary>
        /// <param name="phraseKey">The key of the phrase to get.</param>
        /// <param name="cultureName">The name of the culture to get the phrase in.</param>
        string GetPhrase(string phraseKey, string cultureName);
    }
}