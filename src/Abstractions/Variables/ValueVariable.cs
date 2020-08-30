using System;

namespace Chel.Abstractions.Variables
{
    /// <summary>
    /// A single value variable.
    /// </summary>
    internal class ValueVariable : Variable
    {
        /// <summary>
        /// Gets the value of the variable.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public ValueVariable(string name, string value)
            : base(name)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

                Value = value;
        }
    }
}