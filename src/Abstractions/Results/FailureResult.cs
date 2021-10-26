using System;
using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions.Parsing;

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
        /// Gets the messages describing the failure.
        /// </summary>
        public IReadOnlyList<string> Messages { get; }

        /// <summary>
        /// Create a new instance.
        /// </sumamry>
        /// <param name="sourceLocation">The location where the failure occurred.</param>
        /// <param name="messages">The messages describing the failure.</param>
        public FailureResult(SourceLocation sourceLocation, string[] messages)
        {
            if(messages == null)
                throw new ArgumentNullException(nameof(messages));

            if(messages.Length == 0)
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(messages)), nameof(messages));

            Success = false;
            SourceLocation = sourceLocation;
            Messages = messages;
        }

        public override string ToString()
        {
            var message = string.Format(Texts.ErrorAtLocation, SourceLocation.LineNumber, SourceLocation.CharacterNumber) + ":";
            
            if(Messages.Count == 1)
                message += " " + Messages.First();
            else
                message += Environment.NewLine + string.Join(Environment.NewLine, Messages);

            return message;
        }
    }
}