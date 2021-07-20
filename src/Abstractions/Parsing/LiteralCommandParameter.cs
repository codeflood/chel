using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A literal input parameter for a command.
    /// </summary>
    public class LiteralCommandParameter : CommandParameter
    {
        /// <summary>
        /// Gets the literal value of the parameter.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="value">The literl value of the parameter.</param>
        public LiteralCommandParameter(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static implicit operator LiteralCommandParameter(string value)
        {
            return new LiteralCommandParameter(value);
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || obj.GetType() != typeof(LiteralCommandParameter))
                return false;
            
            var other = (LiteralCommandParameter)obj;
            
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