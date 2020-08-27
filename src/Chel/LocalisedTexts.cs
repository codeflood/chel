using System;
using System.Collections.Generic;
using System.Globalization;
using Chel.Abstractions;

namespace Chel
{
    /// <summary>
    /// A collection of localised text.
    /// </summary>
    public class LocalisedTexts : ITextResolver
    {
        private Dictionary<string, string> _descriptions = new Dictionary<string, string>();
        
        public string GetText(string cultureName)
        {
            if(cultureName == null)
                throw new ArgumentNullException(nameof(cultureName));

            if(_descriptions.ContainsKey(cultureName))
                return _descriptions[cultureName];

            // Try for a less specific culture
            var culture = new CultureInfo(cultureName);

            while(culture != null)
            {
                culture = culture.Parent;

                if(_descriptions.ContainsKey(culture.Name))
                    return _descriptions[culture.Name];

                if(culture == culture.Parent)
                    culture = null;
            }

            return null;
        }

        /// <summary>
        /// Adds the text for a specific culture.
        /// </summary>
        /// <param name="text">The text to add.</param>
        /// <param name="cultureName">The name of the culture the text is for.</param>
        public void AddText(string text, string cultureName)
        {
            if(text == null)
                throw new ArgumentNullException(nameof(text));

            if(string.IsNullOrEmpty(text))
                throw new ArgumentException(string.Format(Texts.ParameterCannotBeEmpty, nameof(text)), nameof(text));

            var key = cultureName ?? CultureInfo.InvariantCulture.Name;

            if(_descriptions.ContainsKey(key))
                throw new InvalidOperationException(string.Format(Texts.TextForCultureAlreadyAdded, key));

            _descriptions.Add(key, text);
        }
    }
}