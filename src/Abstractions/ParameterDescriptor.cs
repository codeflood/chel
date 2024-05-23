using System;
using System.Reflection;

namespace Chel.Abstractions;

/// <summary>
/// A descriptor for a command parameter.
/// </summary>
public abstract class ParameterDescriptor
{
    private ITextResolver _descriptions;

    /// <summary>
    /// Gets the property the parameter will be bound to.
    /// </summary>
    public PropertyDescriptor Property { get; }

    /// <summary>
    /// Gets whether the parameter is required or not.
    /// </summary>
    public bool Required { get; }

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="property">The property the parameter will be bound to.</param>
    /// <param name="descriptions">The descriptions for the parameter.</param>
    /// <param name="required">Indicates whether the parameter is required or not.</param>
    protected ParameterDescriptor(PropertyInfo property, ITextResolver descriptions, bool required)
    {
        if(property == null)
            throw new ArgumentNullException(nameof(property));

        if(descriptions == null)
            throw new ArgumentNullException(nameof(descriptions));

        Property = new PropertyDescriptor(property);
        _descriptions = descriptions;
        Required = required;
    }

    /// <summary>
    /// Gets a description in the specified culture.
    /// </summary>
    public string? GetDescription(string cultureName)
    {
        if(cultureName == null)
            throw new ArgumentNullException(nameof(cultureName));

        return _descriptions.GetText(cultureName);
    }
}
