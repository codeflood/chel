using System;
using System.Collections.Generic;
using System.Linq;

namespace Chel.Abstractions
{
    /// <summary>
    /// The result of parameter binding on a command instance.
    /// </summary>
    public class ParameterBindResult
    {
        private List<string> _errors = null;

        /// <summary>
        /// Indicates whether the binding was successful or not.
        /// </summary>
        public bool Success => !_errors.Any();

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public IReadOnlyList<string> Errors => _errors.AsReadOnly();

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public ParameterBindResult()
        {
            _errors = new List<string>();
        }

        /// <summary>
        /// Adds an error to the parameter binding result.
        /// </summary>
        /// <param name="error">The error to add.</param>
        public void AddError(string error)
        {
            if(error == null)
                throw new ArgumentNullException(nameof(error));

            if(error.Equals(string.Empty))
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(error)), nameof(error));

            _errors.Add(error);
        }
    }
}