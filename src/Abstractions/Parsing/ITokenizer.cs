namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// Defines the tokenizer which emits tokens from the text input.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Indicates whether the tokenizer contains any more tokens.
        /// </summary>
        bool HasMore { get; }

        /// <summary>
        /// Gets the next token available.
        /// </summary>
        /// <returns>The next token available, or null if there are no more tokens.</returns>
        Token GetNextToken();
    }
}