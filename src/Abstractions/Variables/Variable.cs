using System;

namespace Chel.Abstractions.Variables
{
    /// <summary>
    /// Base class for variables.
    /// </summary>
    internal abstract class Variable
    {
        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        public Variable(string name)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if(string.IsNullOrEmpty(name))
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeNullOrEmpty, nameof(name)), nameof(name));

            Name = name;
        }
    }
}