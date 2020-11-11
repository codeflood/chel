namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// Tokenizes textual command input.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Get the next token from the input.
        /// </summary>
        /// <returns>The next token from the input, or null if there is no more input to process.</returns>
        Token GetNextToken();
    }
}
