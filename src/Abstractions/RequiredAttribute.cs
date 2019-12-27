using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Denotes a parameter is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredAttribute : Attribute
    {
    }
}