using System;
using System.Collections.Generic;

namespace Chel.Abstractions
{
    /// <summary>
    /// Describes a command.
    /// </summary>
    public class CommandDescriptor
    {
        private ITextResolver _descriptions = null;

        /// <summary>
        /// Gets the <see cref="Type"/> implementing the command.
        /// </summary>
        public Type ImplementingType { get; private set; }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string CommandName { get; private set; }

        /// <summary>
        /// Gets the numbered parameters of the command.
        /// </summary>
        public IReadOnlyList<NumberedParameterDescriptor> NumberedParameters { get; private set; }

        /// <summary>
        /// Gets the named parameters of the command.
        /// </summary>
        public IReadOnlyDictionary<string, NamedParameterDescriptor> NamedParameters { get; private set; }

        /// <summary>
        /// Gets the flag parameters of the command.
        /// </summary>
        public IReadOnlyList<FlagParameterDescriptor> FlagParameters { get; private set; }

        private CommandDescriptor(string commandName, Type implementingType, ITextResolver descriptions)
        {
            CommandName = commandName;
            ImplementingType = implementingType;
            _descriptions = descriptions;
        }

        /// <summary>
        /// Gets a description in the specified culture.
        /// </summary>
        public string GetDescription(string cultureName)
        {
            if(cultureName == null)
                throw new ArgumentNullException(nameof(cultureName));

            if(_descriptions == null)
                return string.Empty;

            return _descriptions.GetText(cultureName);
        }

        /// <summary>
        /// Builds instances of <see cref="CommandDescriptor" />.
        /// </summary>
        public class Builder
        {
            private string _commandName = null;
            private Type _implementingType = null;
            private ITextResolver _descriptions = null;
            private List<NumberedParameterDescriptor> _numberedParameters = new List<NumberedParameterDescriptor>();
            private Dictionary<string, NamedParameterDescriptor> _namedParameters = new Dictionary<string, NamedParameterDescriptor>(StringComparer.OrdinalIgnoreCase);
            private List<FlagParameterDescriptor> _flagParameters = new List<FlagParameterDescriptor>();

            /// <summary>
            /// Create a new instance.
            /// </summary>
            /// <param name="commandName">The name of the command.</param>
            /// <param name="implementingType">The <see cref="Type"/> implementing the command.</param>
            /// <param name="descriptions">The localised descriptions for the command.</param>
            public Builder(string commandName, Type implementingType, ITextResolver descriptions)
            {
                if(commandName == null)
                    throw new ArgumentNullException(nameof(commandName));

                if(commandName == string.Empty)
                    throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(commandName)), nameof(commandName));

                if(implementingType == null)
                    throw new ArgumentNullException(nameof(implementingType));

                if(descriptions == null)
                    throw new ArgumentNullException(nameof(descriptions));

                _commandName = commandName;
                _implementingType = implementingType;
                _descriptions = descriptions;
            }

            /// <summary>
            /// Adds a numbered parameter descriptor.
            /// </summary>
            /// <param name="descriptor">The descriptor to add.</param>
            public void AddNumberedParameter(NumberedParameterDescriptor descriptor)
            {
                if(descriptor == null)
                    throw new ArgumentNullException(nameof(descriptor));

                var existing = _numberedParameters.Find(x => x.Number == descriptor.Number);
                if(existing != null)
                    throw new InvalidOperationException(string.Format(Texts.DescriptorAlreadyAdded, existing.Number));

                _numberedParameters.Add(descriptor);
            }

            /// <summary>
            /// Adds a named parameter descriptor.
            /// </summary>
            /// <param name="descriptor">The descriptor to add.</param>
            public void AddNamedParameter(NamedParameterDescriptor descriptor)
            {
                if(descriptor == null)
                    throw new ArgumentNullException(nameof(descriptor));

                if(_namedParameters.ContainsKey(descriptor.Name))
                    throw new InvalidOperationException(string.Format(Texts.DescriptorAlreadyAdded, descriptor.Name));

                _namedParameters.Add(descriptor.Name, descriptor);
            }

            /// <summary>
            /// Adds a flag parameter descriptor.
            /// </summary>
            /// <param name="descriptor">The descriptor to add.</param>
            public void AddFlagParameter(FlagParameterDescriptor descriptor)
            {
                if(descriptor == null)
                    throw new ArgumentNullException(nameof(descriptor));

                var found = _flagParameters.Exists(x => string.Equals(x.Name, descriptor.Name, StringComparison.OrdinalIgnoreCase));
                if(found)
                    throw new InvalidOperationException(string.Format(Texts.DescriptorAlreadyAdded, descriptor.Name));

                _flagParameters.Add(descriptor);
            }

            /// <summary>
            /// Builds a <see cref="CommandDescriptor" /> instance from the set data.
            /// </summary>
            public CommandDescriptor Build()
            {
                var descriptor = new CommandDescriptor(_commandName, _implementingType, _descriptions)
                {
                    NumberedParameters = new List<NumberedParameterDescriptor>(_numberedParameters),
                    NamedParameters = new Dictionary<string, NamedParameterDescriptor>(_namedParameters),
                    FlagParameters = new List<FlagParameterDescriptor>(_flagParameters)
                };

                return descriptor;
            }
        }
    }
}