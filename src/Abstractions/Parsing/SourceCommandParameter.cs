namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A command parameter which has been parsed from source.
    /// </summary>
    public abstract class SourceCommandParameter
    {
        /// <summary>
        /// Gets the location the value was parsed from.
        /// </summary>
        public SourceLocation SourceLocation { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="sourceLocation">The location the value was parsed from.</param>
        public SourceCommandParameter(SourceLocation sourceLocation)
        {
            SourceLocation = sourceLocation;
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
                return false;
            
            var other = obj as SourceCommandParameter;

            if (other == null)
                return false;
            
            return SourceLocation.Equals(other.SourceLocation);
        }
        
        public override int GetHashCode()
        {
            return SourceLocation.GetHashCode();
        }
    }
}
