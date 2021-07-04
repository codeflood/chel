namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// Defines the tokenizer which emits tokens from the text input.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Gets the next token available.
        /// </summary>
        /// <returns>The next token available, or null if there are no more tokens.</returns>
        Token GetNextToken();
    }
}