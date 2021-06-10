namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// Pop the state from the top of the tokenizer state stack.
    /// </summary>
    public class PopStateResponse : TokenizerStateResponse
    {
        /// <summary>
        /// Gets the next character to continue processing with.
        /// </summary>
        public char NextChar { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="nextChar">The next character to continue processing with.</param>
        public PopStateResponse(char nextChar)
        {
            NextChar = nextChar;
        }
    }
}
