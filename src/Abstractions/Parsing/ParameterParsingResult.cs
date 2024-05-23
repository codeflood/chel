using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// The result of parsing a parameter for a command.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class ParameterParsingResult<T>
    {
        /// <summary>
        /// Gets the error message for the result, if it was not successful.
        /// </summary>
        public string? ErrorMessage { get; } = null;

        /// <summary>
        /// Gets the value of the result, if it was successful.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Indicates whether the result is successful.
        /// </summary>
        public bool HasError => ErrorMessage != null;

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <param name="value">The value of the result.</param>
        /// <remarks>This is a successful result.</remarks>
        public ParameterParsingResult(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new unsuccessful result.
        /// </summary>
        /// <param name="errorMessage">The error message for the result.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="errorMessage"/> is null.</exception>
        public ParameterParsingResult(string errorMessage)
        {
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }
    }
}
