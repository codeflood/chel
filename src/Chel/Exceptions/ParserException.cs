using System;

namespace Chel.Exceptions
{
    /// <summary>
    /// An <see cref="Exception" /> indicating an error occurred while parsing input.
    /// </summary>
    public class ParserException : Exception
    {
        /// <summary>
        /// Gets the source line where the exception was thrown.
        /// </summary>
        public int SourceLine { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="sourceLine">The source line where the exception was thrown.</param>
        /// <param name="message">A message indicating the cause of the exception.</param>
        public ParserException(int sourceLine, string message)
            : base(message)
        {
            SourceLine = sourceLine;
        }
    }
}