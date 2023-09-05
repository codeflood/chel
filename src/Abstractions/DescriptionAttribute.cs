using System;
using System.Globalization;

namespace Chel.Abstractions
{
    /// <summary>
    /// Provides descriptions for command members.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// Gets the text of the description.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the name of the culture the description is for.
        /// </summary>
        public string CultureName { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="text">The text of the description.</param>
        public DescriptionAttribute(string text)
            : this(text, null)
        {
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="text">The text of the description.</param>
        /// <param name="cultureName">The name of the culture the description is for.</param>
        public DescriptionAttribute(string text, string cultureName)
        {
            if(text == null)
                throw new ArgumentNullException(nameof(text));

            if(string.IsNullOrEmpty(text))
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeEmpty, nameof(text), nameof(text));

            Text = text;
            CultureName = cultureName ?? CultureInfo.InvariantCulture.Name;
        }
    }
}