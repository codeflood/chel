using System;
using Chel.Abstractions;

namespace Chel.Exceptions
{
    /// <summary>
    /// An <see cref="Exception" /> indicating an error occurred while parsing input.
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Gets the source location where the exception was thrown.
        /// </summary>
        public SourceLocation SourceLocation { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="sourceLocation">The source location where the exception was thrown.</param>
        /// <param name="message">A message indicating the cause of the exception.</param>
        public ParseException(SourceLocation sourceLocation, string message)
            : base(message)
        {
            SourceLocation = sourceLocation;
        }
    }
}