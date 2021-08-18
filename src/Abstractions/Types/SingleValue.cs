using System;

namespace Chel.Abstractions.Types
{
    /// <summary>
    /// A single value.
    /// </summary>
    public class SingleValue : ChelType
    {
        /// <summary>
        /// The value of the instance.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="value">The literl value of the parameter.</param>
        public SingleValue(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || obj.GetType() != typeof(SingleValue))
                return false;
            
            var other = (SingleValue)obj;
            
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