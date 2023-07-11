using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Defines an error in the input source.
    /// </summary>
    public record SourceError
    {
        /// <summary>
        /// The location the error occurred at.
        /// </summary>
        public SourceLocation SourceLocation { get; init; }

        /// <summary>
        /// A message explaining the error.
        /// </summary>
        public string Message { get; init;}

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="sourceLocation">The location the error occurred at.</param>
        /// <param name="message">A message explaining the error.</param>
        /// <exception cref="ArgumentNullException"><paramref="message"/> cannot be null.</exception>
        /// <exception cref="ArgumentException"><paramref="message"/> cannot be empty or whitespace.</exception>
        public SourceError(SourceLocation sourceLocation, string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            if(string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmptyOrWhitespace, nameof(message)), nameof(message));
            
            SourceLocation = sourceLocation;
        }
    }
}