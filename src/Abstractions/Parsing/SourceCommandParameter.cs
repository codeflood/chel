using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A command parameter which has been parsed from source.
    /// </summary>
    public class SourceCommandParameter
    {
        /// <summary>
        /// Gets the value of the command parameter.
        /// </summary>
        public ICommandParameter Value { get; }

        /// <summary>
        /// Gets the location the value was parsed from.
        /// </summary>
        public SourceLocation SourceLocation { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="value">The value of the command parameter.</param>
        /// <param name="value">The location the value was parsed from.</param>
        public SourceCommandParameter(ICommandParameter value, SourceLocation sourceLocation)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            SourceLocation = sourceLocation;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(SourceCommandParameter))
            {
                return false;
            }

            var other = (SourceCommandParameter)obj;
            
            return
                Value.Equals(other.Value) &&
                SourceLocation.Equals(other.SourceLocation);
        }
        
        public override int GetHashCode()
        {
            return (Value, SourceLocation).GetHashCode();
        }
    }
}
