using System.Collections.Generic;

namespace Chel.Abstractions
{
    /// <summary>
    /// Resolves application texts by key.
    /// </summary>
    public class ApplicationTextResolver
    {
        private static object _instanceCreationLock = new();
        private static ApplicationTextResolver _instance;

        private Dictionary<string, string> _defaultTexts;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static ApplicationTextResolver Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_instanceCreationLock)
                    {
                        if(_instance == null)
                            _instance = new ApplicationTextResolver();
                    }
                }

                return _instance;
            }
        }

        private ApplicationTextResolver()
        {
            _defaultTexts = new()
            {
                { ApplicationTexts.ArgumentCannotBeEmpty, "'{0}' cannot be empty" },
                { ApplicationTexts.ArgumentCannotBeEmptyOrWhitespace, "'{0}' cannot be empty or whitespace" },
                { ApplicationTexts.ArgumentCannotBeNull, "'{0}' cannot be null" },
                { ApplicationTexts.ArgumentNotValidCulture, "'{0}' is not a valid culture" },
                { ApplicationTexts.ArgumentMustBeGreaterThanZero, "'{0}' must be greater than 0" },
                { ApplicationTexts.ArgumentCannotBeNullOrEmpty, "'{0}' cannot be null or empty" },
                { ApplicationTexts.CompoundValueOnlyConsistsLiteralsAndVariables, "Compound values can only consist of literals and variable references." },
                { ApplicationTexts.DescriptorAlreadyAdded, "Descriptor '{0}' has already been added." },
                { ApplicationTexts.ErrorAtLocation, "ERROR (line {0}, character {1})" },
                { ApplicationTexts.FlagParametersCannotBeRequired, "Flag parameters cannot be required" },
                { ApplicationTexts.InvalidCommandNameNull, "The command name null is invalid" },
                { ApplicationTexts.InvalidCommandNameWithParameter, "The command name '{0}' is invalid" }
            };
        }

        /// <summary>
        /// Resolve text by key for a given culture.
        /// </summary>
        /// <param name="key">The key of the text to resolve.</param>
        /// <param name="cultureName">The name of the culture to resolve the text in.</param>
        public string Resolve(string key, string cultureName)
        {
            if(string.IsNullOrEmpty(key))
                return string.Empty;
            
            // todo: Add culture overloading from files
            var effectiveCultureName = cultureName ?? string.Empty;

            if(_defaultTexts.ContainsKey(key))
                return _defaultTexts[key];
            
            return key;
        }
    }
}
