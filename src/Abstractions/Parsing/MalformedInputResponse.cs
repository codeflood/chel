using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// Indicates the input was malformed.
    /// </summary>
    public class MalformedInputResponse : TokenizerStateResponse
    {
        /// <summary>
        /// Gets the message describing how the input is malformed.
        /// </summary>
        public string Message { get; }

        public MalformedInputResponse(string message)
        {
            if(message == null)
                throw new ArgumentNullException(nameof(message));

            if(string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));

            Message = message;
        }
    }
}