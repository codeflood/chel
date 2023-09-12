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
        /// Gets the placeholder text for the parameter value.
        /// </summary>
        public string ValuePlaceholderText { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="name">The name of the named parameter to bind.</param>
        /// <param name="valuePlaceholderText">The placeholder text for the parameter value.</param>
        public NamedParameterAttribute(string name, string valuePlaceholderText)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if(string.IsNullOrWhiteSpace(name))
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeEmptyOrWhitespace, nameof(name), nameof(name));

            if(valuePlaceholderText == null)
                throw new ArgumentNullException(nameof(valuePlaceholderText));

            if(string.IsNullOrWhiteSpace(valuePlaceholderText))
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeEmptyOrWhitespace, nameof(valuePlaceholderText), nameof(valuePlaceholderText));

            Name = name;
            ValuePlaceholderText = valuePlaceholderText;
        }
    }
}