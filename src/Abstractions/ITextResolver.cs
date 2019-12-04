namespace Chel.Abstractions
{
    /// <summary>
    /// Resolves some text for a specific language.
    /// </summary>
    public interface ITextResolver
    {
        /// <summary>
        /// Gets text in the specified culture.
        /// </summary>
        /// <param name="cultureName">The name of the culture to get the text in.</param>
        string GetText(string cultureName);
    }
}