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
        private readonly List<SourceError> _errors = null;

        /// <summary>
        /// Indicates whether the binding was successful or not.
        /// </summary>
        public bool Success => !_errors.Any();

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public IReadOnlyList<SourceError> Errors => _errors.AsReadOnly();

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public ParameterBindResult()
        {
            _errors = new List<SourceError>();
        }

        /// <summary>
        /// Adds an error to the parameter binding result.
        /// </summary>
        /// <param name="error">The error to add.</param>
        public void AddError(SourceError error)
        {
            if(error == null)
                throw new ArgumentNullException(nameof(error));

            if(error.Equals(string.Empty))
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(error)), nameof(error));

            _errors.Add(error);
        }
    }
}