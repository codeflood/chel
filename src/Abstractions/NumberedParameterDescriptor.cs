using System;
using System.Reflection;

namespace Chel.Abstractions
{
    /// <summary>
    /// A descriptor for a numbered parameter.
    /// </summary>
    public class NumberedParameterDescriptor : ParameterDescriptor
    {
        /// <summary>
        /// Gets the number of the numbered parameter.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Gets the placeholder text for the parameter.
        /// </summary>
        public string PlaceholderText { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="number">The number of the numbered parameter.</param>
        /// <param name="placeholderText">The placeholder text for the parameter.</param>
        /// <param name="property">The property the parameter will be bound to.</param>
        /// <param name="descriptions">The descriptions for the parameter.</param>
        /// <param name="required">Indicates whether the parameter is required or not.</param>
        public NumberedParameterDescriptor(int number, string placeholderText, PropertyInfo property, ITextResolver descriptions, bool required)
            : base(property, descriptions, required)
        {
            if(number <= 0)
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentMustBeGreaterThanZero, nameof(number), nameof(number));

            if(placeholderText == null)
                throw new ArgumentNullException(nameof(placeholderText));

            if(placeholderText.Equals(string.Empty))                
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeEmpty, nameof(placeholderText), nameof(placeholderText));

            Number = number;
            PlaceholderText = placeholderText;
        }
    }
}