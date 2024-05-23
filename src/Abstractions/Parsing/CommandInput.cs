using System;
using System.Collections.Generic;
using Chel.Abstractions.Types;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A single command input to be executed.
    /// </summary>
    public class CommandInput : SourceCommandParameter
    {
        internal List<ICommandParameter> _parameters = new();

        /// <summary>
        /// Gets the identifier of the command to execute.
        /// </summary>
        public ExecutionTargetIdentifier CommandIdentifier { get; private set; }

        /// <summary>
        /// Gets or sets a value to substitute in place of this command input.
        /// </summary>
        public ChelType? SubstituteValue { get; set; }

        /// <summary>
        /// Gets the parameters for the command.
        /// </summary>
        public IReadOnlyList<ICommandParameter> Parameters => _parameters;

        private CommandInput(SourceLocation sourceLocation)
            : base(sourceLocation)
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
                CommandIdentifier.Equals(other.CommandIdentifier) &&
                SourceLocation.Equals(other.SourceLocation) &&
                parametersEqual;
        }

        public override int GetHashCode()
        {
            var hashCode = (CommandIdentifier, SourceLocation).GetHashCode();

            unchecked
            {
                foreach(var p in Parameters)
                    hashCode = hashCode * 3067 + p.GetHashCode(); // multiply by prime number
            }

            return hashCode;
        }

        /// <summary>
        /// Builds instances of <see cref="CommandInput" />.
        /// </summary>
        public class Builder
        {
            private readonly CommandInput _commandInput;

            /// <summary>
            /// Create a new instance.
            /// </summary>
            /// <param name="sourceLocation">The location the command was parsed from.</param>
            /// <param name="commandIdentifier">The identifier of the command to execute.</param>
            public Builder(SourceLocation sourceLocation, ExecutionTargetIdentifier commandIdentifier)
            {
                if(commandIdentifier.Name.Equals(string.Empty))
                    throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeEmpty, nameof(commandIdentifier.Name), nameof(commandIdentifier.Name));

                _commandInput = new CommandInput(sourceLocation)
                {
                    CommandIdentifier = commandIdentifier
                };
            }

            /// <summary>
            /// Add a parameter to the command input.
            /// </summary>
            /// <param name="value">The parameter value to add.</param>
            public void AddParameter(ICommandParameter value)
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(value));

                _commandInput._parameters.Add(value);
            }

            /// <summary>
            /// Builds a <see cref="CommandInput" /> instance from the set data.
            /// </summary>
            public CommandInput Build()
            {
                return _commandInput;
            }
        }
    }
}
