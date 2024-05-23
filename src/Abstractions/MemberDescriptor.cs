using System;
using System.Collections.Generic;
using System.Globalization;

namespace Chel.Abstractions;

/// <summary>
/// Describes a member of a command.
/// </summary>
public abstract class MemberDescriptor
{
    private Dictionary<string, string> _descriptions = new Dictionary<string, string>();

    protected MemberDescriptor()
    {
    }

    /// <summary>
    /// Gets a description in the specified culture.
    /// </summary>
    public string? GetDescription(string cultureName)
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

    protected void SetDescriptions(Dictionary<string, string> descriptions)
    {
        _descriptions = new Dictionary<string, string>(descriptions);
    }

    /// <summary>
    /// Builds instances of <see cref="MemberDescriptor" />.
    /// </summary>
    /// <typeparam name="T">The type the builder builds.</typeparam>
    public abstract class MemberDescriptorBuilder<T> where T : MemberDescriptor, new()
    {
        private Dictionary<string, string> _descriptions = new Dictionary<string, string>();
        
        /// <summary>
        /// Add a description to the descriptor.
        /// </summary>
        /// <param name="description">The description to add.</param>
        /// <param name="cultureName">The name of the culture the description is for.</param>
        public void AddDescription(string description, string cultureName)
        {
            if(description == null)
                throw new ArgumentNullException(nameof(description));

            if(string.IsNullOrEmpty(description))
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeEmpty, nameof(description), nameof(description));

            var key = cultureName ?? CultureInfo.InvariantCulture.Name;

            

            _descriptions.Add(key, description);
        }

        /// <summary>
        /// Build the <see cref="T" /> using the information set on the builder.
        /// </summary>
        public virtual T Build()
        {
            var descriptor = new T();

            descriptor.SetDescriptions(_descriptions);

            return descriptor;
        }
    }
}
