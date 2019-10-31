using System;
using System.Collections.Generic;
using System.Linq;

namespace Chel.Abstractions.Results
{
    /// <summary>
    /// A failed <see cref="CommandResult" />.
    /// </summary>
    public class FailureResult : CommandResult
    {
        /// <summary>
        /// Gets the line on which the failure occurred.
        /// </summary>
        public int SourceLine { get; }

        /// <summary>
        /// Gets the messages describing the failure.
        /// </summary>
        public IReadOnlyList<string> Messages { get; }

        /// <summary>
        /// Create a new instance.
        /// </sumamry>
        /// <param name="sourceLine">The line on which the failure occurred.</param>
        /// <param name="messages">The messages describing the failure.</param>
        public FailureResult(int sourceLine, string[] messages)
        {
            if(messages == null)
                throw new ArgumentNullException(nameof(messages));

            if(messages.Length == 0)
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(messages)), nameof(messages));

            Success = false;
            SourceLine = sourceLine;
            Messages = messages;
        }

        public override string ToString()
        {
            var message = string.Format(Texts.ErrorOnLine, SourceLine) + ":";
            
            if(Messages.Count == 1)
                message += " " + Messages.First();
            else
                message += Environment.NewLine + string.Join(Environment.NewLine, Messages);

            return message;
        }
    }
}