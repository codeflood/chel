using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Denotes a property is a binding target of a named parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NamedParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the named parameter to bind.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="name">The name of the named parameter to bind.</param>
        public NamedParameterAttribute(string name)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name) + " cannot be empty or whitespace", nameof(name));

            Name = name;
        }
    }
}