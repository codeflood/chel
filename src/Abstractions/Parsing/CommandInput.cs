using System;
using System.Collections.Generic;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A single command input to be executed.
    /// </summary>
    public class CommandInput
    {
        /// <summary>
        /// Gets the line number the command was parsed from.
        /// </summary>
        public int SourceLine { get; private set; }

        /// <summary>
        /// Gets the name of the command to execute.
        /// </summary>
        public string CommandName { get; private set; }

        /// <summary>
        /// Gets the parameters for the command.
        /// </summary>
        public IReadOnlyList<CommandParameter> Parameters { get; private set; }

        private CommandInput()
        {
        }

        /// <summary>
        /// Builds instances of <see cref="CommandInput" />.
        /// </summary>
        public class Builder
        {
            private int _sourceLine = -1;
            private string _commandName = null;
            private List<CommandParameter> _parameters = null;

            /// <summary>
            /// Create a new instance.
            /// </summary>
            /// <param name="sourceLine">The line number the command was parsed from.</param>
            /// <param name="commandName">The name of the command to execute.</param>
            public Builder(int sourceLine, string commandName)
            {
                if(sourceLine <= 0)
                    throw new ArgumentException(string.Format(Texts.ArgumentMustBeGreaterThanZero, nameof(sourceLine)), nameof(sourceLine));

                if(commandName == null)
                    throw new ArgumentNullException(nameof(commandName));

                if(commandName.Equals(string.Empty))
                    throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(commandName)), nameof(commandName));

                _sourceLine = sourceLine;
                _commandName = commandName;

                _parameters = new List<CommandParameter>();
            }

            /// <summary>
            /// Add a parameter to the command input.
            /// </summary>
            /// <param name="value">The parameter value to add.</param>
            public void AddParameter(CommandParameter value)
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(value));

                _parameters.Add(value);
            }

            /// <summary>
            /// Builds a <see cref="CommandInput" /> instance from the set data.
            /// </summary>
            public CommandInput Build()
            {
                var commandInput = new CommandInput()
                {
                    SourceLine = _sourceLine,
                    CommandName = _commandName,
                    Parameters = new List<CommandParameter>(_parameters),
                };

                return commandInput;
            }
        }
    }
}