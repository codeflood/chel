using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Denotes a property is a binding target of a flag parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FlagParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the flag parameter to bind.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="name">The name of the flag parameter to bind.</param>
        public FlagParameterAttribute(string name)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if(string.IsNullOrWhiteSpace(name))
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeEmptyOrWhitespace, nameof(name), nameof(name));

            Name = name;
        }
    }
}