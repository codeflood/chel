using System;
using Chel.Abstractions;

namespace Chel.Exceptions
{
    /// <summary>
    /// An <see cref="Exception" /> indicating an unset variable was used.
    /// </summary>
    public class UnsetVariableException : Exception
    {
        /// <summary>
        /// Gets the name of the variable that is not set.
        /// </summary>
        public string VariableName { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="variableName">The name of the variable that is not set.</param>
        public UnsetVariableException(string variableName)
            : base(string.Format(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.VariableIsUnset), variableName))
        {
            if(variableName == null)
                throw new ArgumentNullException(nameof(variableName));

            if(string.IsNullOrWhiteSpace(variableName))
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeEmptyOrWhitespace, nameof(variableName), nameof(variableName));

            VariableName = variableName;
        }
    }
}