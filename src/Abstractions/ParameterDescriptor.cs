using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chel.Abstractions
{
    /// <summary>
    /// A descriptor for a command parameter.
    /// </summary>
    public abstract class ParameterDescriptor
    {
        private ITextResolver _descriptions = null;

        /// <summary>
        /// Gets the property the parameter will be bound to.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="property">The property the parameter will be bound to.</param>
        /// <param name="descriptions">The descriptions for the parameter.</param>
        protected ParameterDescriptor(PropertyInfo property, ITextResolver descriptions)
        {
            if(property == null)
                throw new ArgumentNullException(nameof(property));

            if(descriptions == null)
                throw new ArgumentNullException(nameof(descriptions));

            Property = property;
            _descriptions = descriptions;
        }

        /// <summary>
        /// Gets a description in the specified culture.
        /// </summary>
        public string GetDescription(string cultureName)
        {
            if(cultureName == null)
                throw new ArgumentNullException(nameof(cultureName));

            return _descriptions.GetText(cultureName);
        }
    }
}