using System;
using System.Collections.Generic;
using Chel.Abstractions.Types;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A single command input to be executed.
    /// </summary>
    public class CommandInput : ICommandParameter
    {
        /// <summary>
        /// Gets the location the command was parsed from.
        /// </summary>
        public SourceLocation SourceLocation { get; private set; }

        /// <summary>
        /// Gets the name of the command to execute.
        /// </summary>
        public string CommandName { get; private set; }

        /// <summary>
        /// Gets or sets a value to substitute in place of this command input.
        /// </summary>
        public ChelType SubstituteValue { get; set; }

        /// <summary>
        /// Gets the parameters for the command.
        /// </summary>
        public IReadOnlyList<ICommandParameter> Parameters { get; private set; }

        private CommandInput()
        {
        }

        public override bool Equals(object obj)
        {
            var other = obj as CommandInput;

            if(other == null)
                return false;

            var parametersEqual = true;

            if(!Parameters.Count.Equals(other.Parameters.Count))
                return false;

            for(var i = 0; i < Parameters.Count; i++)
            {
                parametersEqual = Parameters[i].Equals(other.Parameters[i]);

                if(!parametersEqual)
                    break;
            }

            return
                CommandName.Equals(other.CommandName) &&
                SourceLocation.Equals(other.SourceLocation) &&
                parametersEqual;
        }

        

        public override int GetHashCode()
        {
            var hashCode = 0;

            foreach(var p in Parameters)
                hashCode += p.GetHashCode();

            return
                CommandName.GetHashCode() +
                SourceLocation.GetHashCode() +
                hashCode;
        }

        /// <summary>
        /// Builds instances of <see cref="CommandInput" />.
        /// </summary>
        public class Builder
        {
            private SourceLocation _sourceLocation;
            private string _commandName = null;
            private List<ICommandParameter> _parameters = null;

            /// <summary>
            /// Create a new instance.
            /// </summary>
            /// <param name="sourceLocation">The location the command was parsed from.</param>
            /// <param name="commandName">The name of the command to execute.</param>
            public Builder(SourceLocation sourceLocation, string commandName)
            {
                if(commandName == null)
                    throw new ArgumentNullException(nameof(commandName));

                if(commandName.Equals(string.Empty))
                    throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(commandName)), nameof(commandName));

                _sourceLocation = sourceLocation;
                _commandName = commandName;

                _parameters = new List<ICommandParameter>();
            }

            /// <summary>
            /// Add a parameter to the command input.
            /// </summary>
            /// <param name="value">The parameter value to add.</param>
            public void AddParameter(ICommandParameter value)
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
                    SourceLocation = _sourceLocation,
                    CommandName = _commandName,
                    Parameters = new List<ICommandParameter>(_parameters),
                };

                return commandInput;
            }
        }
    }
}
