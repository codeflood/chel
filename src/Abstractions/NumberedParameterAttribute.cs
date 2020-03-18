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
        /// Gets the placeholder text for the parameter.
        /// </summary>
        public string PlaceholderText { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="number">The number of the numbered parameter to bind.</param>
        /// <param name="placeholderText">The placeholder text for the parameter.</param>
        public NumberedParameterAttribute(int number, string placeholderText)
        {
            if(number <= 0)
                throw new ArgumentException(string.Format(Texts.ArgumentMustBeGreaterThanZero, nameof(number)), nameof(number));

            if(placeholderText == null)
                throw new ArgumentNullException(nameof(placeholderText));

            if(string.IsNullOrWhiteSpace(placeholderText))
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmptyOrWhitespace, nameof(placeholderText)), nameof(placeholderText));

            Number = number;
            PlaceholderText = placeholderText;
        }
    }
}