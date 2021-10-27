using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A single token emitted from the tokenizer.
    /// </summary>
    public abstract class Token
    {
        /// <summary>
        /// Gets the location in the source where the token was parsed from.
        /// </summary>
        public SourceLocation Location { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="location">The location in the source where the token was parsed from.</param>
        public Token(SourceLocation location)
        {
            Location = location;
        }
    }
}