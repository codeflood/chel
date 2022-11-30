using System;
using Chel.Abstractions.Types;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A command parameter which has been parsed from source and contains a value.
    /// </summary>
    public class SourceValueCommandParameter : SourceCommandParameter
    {
        /// <summary>
        /// Gets the value of the command parameter.
        /// </summary>
        public ChelType Value { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="sourceLocation">The location the value was parsed from.</param>
        /// <param name="value">The value of the command parameter.</param>
        public SourceValueCommandParameter(SourceLocation sourceLocation, ChelType value)
            : base(sourceLocation)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override bool Equals(object obj)
        {
            if(!base.Equals(obj))
                return false;

            if (obj == null || obj.GetType() != typeof(SourceValueCommandParameter))
            {
                return false;
            }

            var other = (SourceValueCommandParameter)obj;
            
            return
                // SourceLocation is handled in the base class.
                Value.Equals(other.Value);
        }
        
        public override int GetHashCode()
        {
            var code = 23;
            unchecked
            {
                code += base.GetHashCode();
                code += Value.GetHashCode();
            }

            return code;
        }
    }
}
