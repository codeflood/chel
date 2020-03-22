using System;
using System.Reflection;

namespace Chel.Abstractions
{
    /// <summary>
    /// A descriptor for a named parameter.
    /// </summary>
    public class NamedParameterDescriptor : ParameterDescriptor
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the placeholder text for the parameter value.
        /// </summary>
        public string ValuePlaceholderText { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="valuePlaceholderText">The placeholder text for the parameter value.</param>
        /// <param name="property">The property the parameter will be bound to.</param>
        /// <param name="descriptions">The descriptions for the parameter.</param>
        /// <param name="required">Indicates whether the parameter is required or not.</param>
        public NamedParameterDescriptor(string name, string valuePlaceholderText, PropertyInfo property, ITextResolver descriptions, bool required)
            : base(property, descriptions, required)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmptyOrWhitespace, nameof(name)), nameof(name));

            if(valuePlaceholderText == null)
                throw new ArgumentNullException(nameof(valuePlaceholderText));

            if(string.IsNullOrWhiteSpace(valuePlaceholderText))
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmptyOrWhitespace, nameof(valuePlaceholderText)), nameof(valuePlaceholderText));

            Name = name;
            ValuePlaceholderText = valuePlaceholderText;
        }
    }
}