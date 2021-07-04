namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A literal token with no special meaning.
    /// </summary>
    public class SpecialToken : Token
    {
        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        public SpecialTokenType Type { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="location">The location in the source where the token was parsed from.</param>
        /// <param name="type">The type of the token.</param>
        public SpecialToken(SourceLocation location, SpecialTokenType type)
            : base(location)
        {
            Type = type;
        }
    }
}
