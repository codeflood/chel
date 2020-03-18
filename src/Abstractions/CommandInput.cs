using System;
using System.Collections.Generic;

namespace Chel.Abstractions
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
        /// Gets the numbered parameters for the command.
        /// </summary>
        public IReadOnlyList<string> NumberedParameters { get; private set; }

        /// <summary>
        /// Gets the named parameters for the command.
        /// </summary>
        public IReadOnlyDictionary<string, string> NamedParameters { get; private set; }

        private CommandInput()
        {
        }

        public override bool Equals(object obj)
        {
            if(obj is CommandInput)
            {
                var other = obj as CommandInput;

                if(NumberedParameters.Count != other.NumberedParameters.Count)
                    return false;

                for(var i = 0; i < NumberedParameters.Count; i++)
                {
                    if(!NumberedParameters[i].Equals(other.NumberedParameters[i]))
                        return false;
                }

                if(NamedParameters.Count != other.NamedParameters.Count)
                    return false;

                foreach(var parameter in NamedParameters)
                {
                    if(!other.NamedParameters.ContainsKey(parameter.Key))
                        return false;

                    if(!other.NamedParameters[parameter.Key].Equals(parameter.Value))
                        return false;
                }

                return
                    SourceLine == other.SourceLine && 
                    string.Compare(CommandName, other.CommandName, true) == 0;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hash = 0;
            foreach(var parameter in NumberedParameters)
            {
                hash += parameter.GetHashCode();
            }

            foreach(var parameter in NamedParameters)
            {
                hash += StringComparer.OrdinalIgnoreCase.GetHashCode(parameter.Key) + parameter.Value.GetHashCode();
            }

            return
                hash +
                SourceLine.GetHashCode() + 
                CommandName.ToLower().GetHashCode();
        }

        /// <summary>
        /// Builds instances of <see cref="CommandInput" />.
        /// </summary>
        public class Builder
        {
            private int _sourceLine = -1;
            private string _commandName = null;
            private List<string> _numberedParameters = null;
            private Dictionary<string, string> _namedParameters = null;

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

                _numberedParameters = new List<string>();
                _namedParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            /// <summary>
            /// Add a numbered parameter to the command input.
            /// </summary>
            /// <param name="value">The parameter value to add.</param>
            public void AddNumberedParameter(string value)
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(value));

                _numberedParameters.Add(value);
            }

            /// <summary>
            /// Add a named parameter to the command input.
            /// </summary>
            /// <param name="name">The name of the parameter to add.</param>
            /// <param name="value">The value of the parameter to add.</param>
            public void AddNamedParameter(string name, string value)
            {
                if(name == null)
                    throw new ArgumentNullException(nameof(name));

                if(string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException(nameof(name) + " cannot be empty or whitespace", nameof(name));

                if(value == null)
                    throw new ArgumentNullException(nameof(value));

                if(_namedParameters.ContainsKey(name))
                    throw new ArgumentException(name + " has already been added", nameof(name));

                _namedParameters.Add(name, value);
            }

            public CommandInput Build()
            {
                var commandInput = new CommandInput()
                {
                    SourceLine = _sourceLine,
                    CommandName = _commandName,
                    NumberedParameters = new List<string>(_numberedParameters),
                    NamedParameters = new Dictionary<string, string>(_namedParameters, StringComparer.OrdinalIgnoreCase)
                };

                return commandInput;
            }
        }
    }
}