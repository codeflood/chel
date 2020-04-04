using System;
using System.Reflection;

namespace Chel.Exceptions
{
    /// <summary>
    /// An <see cref="Exception" /> indicating an invalid parameter definition was encountered.
    /// </summary>
    public class InvalidParameterDefinitionException : Exception
    {
        /// <summary>
        /// Gets the <see cref="PropertyInfo" /> of the parameter exposing the property.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo" /> of the parameter exposing the property.</param>
        /// <param name="message">The message of the exception.</param>
        public InvalidParameterDefinitionException(PropertyInfo property, string message)
            : base(message)
        {
            if(property == null)
                throw new ArgumentNullException(nameof(property));

            Property = property;
        }
    }
}