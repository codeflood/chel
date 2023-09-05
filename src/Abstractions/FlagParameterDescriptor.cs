using System;
using System.Reflection;

namespace Chel.Abstractions
{
    /// <summary>
    /// A descriptor for a flag parameter.
    /// </summary>
    public class FlagParameterDescriptor : ParameterDescriptor
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="property">The property the parameter will be bound to.</param>
        /// <param name="descriptions">The descriptions for the parameter.</param>
        /// <param name="required">Indicates whether the parameter is required or not.</param>
        public FlagParameterDescriptor(string name, PropertyInfo property, ITextResolver descriptions, bool required)
            : base(property, descriptions, required)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if(string.IsNullOrWhiteSpace(name))
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeEmptyOrWhitespace, nameof(name), nameof(name));

            if(required)
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.FlagParametersCannotBeRequired, nameof(required));

            Name = name;
        }
    }
}