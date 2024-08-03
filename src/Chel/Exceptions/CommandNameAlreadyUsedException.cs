using System;
using Chel.Abstractions;

namespace Chel
{
    /// <summary>
    /// An <see cref="Exception" /> indicating a command name has already been used.
    /// </summary>
    public class CommandNameAlreadyUsedException : Exception
    {
        /// <summary>
        /// Gets the name of the command which is already used.
        /// </summary>
        public ExecutionTargetIdentifier CommandIdentifier { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type" /> of the command that caused the exception.
        /// </summary>
        public Type CommandType { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type" /> of the command already registered.
        /// </summary>
        public Type OtherCommandType { get; private set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandName">The name of the command which is already used.</param>
        /// <param name="commandType">The <see cref="Type" /> of the command that caused the exception.</param>
        /// <param name="otherCommandType">The <see cref="Type" /> of the command already registered.</param>
        public CommandNameAlreadyUsedException(ExecutionTargetIdentifier commandIdentifier, Type commandType, Type otherCommandType)
            : base(ApplicationTextResolver.Instance.ResolveAndFormat(
                ApplicationTexts.CommandNameAlreadyUsed,
                commandIdentifier.ToString(),
                commandType?.FullName ?? string.Empty,
                otherCommandType?.FullName ?? string.Empty)
            )
        {
            if(commandType == null)
                throw new ArgumentNullException(nameof(commandType));

            if(otherCommandType == null)
                throw new ArgumentNullException(nameof(otherCommandType));

            CommandIdentifier = commandIdentifier;
            CommandType = commandType;
            OtherCommandType = otherCommandType;
        }
    }
}