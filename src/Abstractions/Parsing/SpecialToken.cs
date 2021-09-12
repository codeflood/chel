namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A special token with implied meaning.
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

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(SpecialToken))
            {
                return false;
            }

            var other = (SpecialToken)obj;
            
            return
                other.Location.Equals(Location) &&
                other.Type.Equals(Type);
        }
        
        public override int GetHashCode()
        {
            return
                Location.GetHashCode() +
                Type.GetHashCode();
        }
    }
}
