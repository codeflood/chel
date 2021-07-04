namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A literal token with no special meaning.
    /// </summary>
    public class LiteralToken : Token
    {
        /// <summary>
        /// Gets the value of the token.
        /// </summary>
        public char Value { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="location">The location in the source where the token was parsed from.</param>
        /// <param name="value">The value of the token.</param>
        public LiteralToken(SourceLocation location, char value)
            : base(location)
        {
            Value = value;
        }
    }
}
