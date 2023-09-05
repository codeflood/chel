using System;
using System.Globalization;

namespace Chel.Abstractions.Results
{
    /// <summary>
    /// A failed <see cref="CommandResult" />.
    /// </summary>
    public class FailureResult : CommandResult
    {
        /// <summary>
        /// Gets the location where the failure occurred.
        /// </summary>
        public SourceLocation SourceLocation { get; }

        /// <summary>
        /// Gets the message describing the failure.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Create a new instance.
        /// </sumamry>
        /// <param name="message">A message describing the failure.</param>
        public FailureResult(string message)
            : this(SourceLocation.CurrentLocation, message)
        {
        }

        /// <summary>
        /// Create a new instance.
        /// </sumamry>
        /// <param name="sourceLocation">The location where the failure occurred.</param>
        /// <param name="message">A messages describing the failure.</param>
        public FailureResult(SourceLocation sourceLocation, string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            if(string.IsNullOrWhiteSpace(message))
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeEmptyOrWhitespace, nameof(message), nameof(message));

            Success = false;
            SourceLocation = sourceLocation;
        }

        public override string ToString()
        {
            var text = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.ErrorAtLocation, CultureInfo.CurrentCulture.Name);
            return string.Format(text, SourceLocation.LineNumber, SourceLocation.CharacterNumber) + ": " + Message;
        }
    }
}