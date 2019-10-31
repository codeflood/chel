using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Denotes a property is a binding target of a numbered parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NumberedParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets the number of the numbered parameter to bind.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="number">The number of the numbered parameter to bind.</param>
        public NumberedParameterAttribute(int number)
        {
            if(number <= 0)
                throw new ArgumentException(string.Format(Texts.ArgumentMustBeGreaterThanZero, nameof(number)), nameof(number));

            Number = number;
        }
    }
}