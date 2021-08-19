using System;

namespace Chel.Abstractions.Types
{
    /// <summary>
    /// A literal value.
    /// </summary>
    public class Literal : ChelType
    {
        /// <summary>
        /// The value of the instance.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="value">The literal value of the instance.</param>
        public Literal(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || obj.GetType() != typeof(Literal))
                return false;
            
            var other = (Literal)obj;
            
            return Value.Equals(other.Value);
        }
        
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}