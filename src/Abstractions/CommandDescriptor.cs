using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Chel.Abstractions
{
    /// <summary>
    /// Describes a command.
    /// </summary>
    public class CommandDescriptor
    {
        private Dictionary<string, string> _descriptions = new Dictionary<string, string>();

        /// <summary>
        /// Gets the <see cref="Type"/> implementing the command.
        /// </summary>
        public Type ImplementingType { get; private set; }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string CommandName { get; private set; }

        private CommandDescriptor()
        {
        }

        public string GetDescription(string cultureName)
        {
            if(cultureName == null)
                throw new ArgumentNullException(nameof(cultureName));

            if(_descriptions.ContainsKey(cultureName))
                return _descriptions[cultureName];

            // Try for a less specific culture
            var culture = new CultureInfo(cultureName);

            while(culture != null)
            {
                culture = culture.Parent;

                if(_descriptions.ContainsKey(culture.Name))
                    return _descriptions[culture.Name];

                if(culture == culture.Parent)
                    culture = null;
            }

            return null;
        }

        public override bool Equals(object obj)
        {
            if(obj is CommandDescriptor)
            {
                var other = obj as CommandDescriptor;

                return
                    ImplementingType.Equals(other.ImplementingType) &&
                    string.Compare(CommandName, other.CommandName, true) == 0;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ImplementingType.GetHashCode() + CommandName.ToLower().GetHashCode();
        }

        private void SetDescriptions(Dictionary<string, string> descriptions)
        {
            _descriptions = new Dictionary<string, string>(descriptions);
        }

        /// <summary>
        /// Builds instances of <see cref="CommandDescriptor" />.
        /// </summary>
        public class Builder
        {
            private Type _implementingType = null;
            private string _commandName = null;
            private Dictionary<string, string> _descriptions = new Dictionary<string, string>();

            /// <summary>
            /// Create a new instance.
            /// </summary>
            /// <param name="implementingType">The <see cref="Type"/> implementing the command.</param>
            /// <param name="commandName">The name of the command.</param>
            public Builder(Type implementingType, string commandName)
            {
                if(implementingType == null)
                    throw new ArgumentNullException(nameof(implementingType));

                if(commandName == null)
                    throw new ArgumentNullException(nameof(commandName));

                if(commandName == string.Empty)
                    throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(commandName)), nameof(commandName));

                _implementingType = implementingType;
                _commandName = commandName;
            }

            /// <summary>
            /// Add a description to the descriptor.
            /// </summary>
            /// <param name="description">The description to add.</param>
            /// <param name="cultureName">The name of the culture the description is for.</param>
            public void AddDescription(string description, string cultureName)
            {
                if(description == null)
                    throw new ArgumentNullException(nameof(description));

                if(string.IsNullOrEmpty(description))
                    throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(description)), nameof(description));

                var key = cultureName ?? CultureInfo.InvariantCulture.Name;

                if(_descriptions.ContainsKey(key))
                    throw new InvalidOperationException(string.Format(Texts.DescriptionForCultureAlreadyAdded, key));

                _descriptions.Add(key, description);
            }

            /// <summary>
            /// Build the <see cref="CommandDescriptor" /> using the information set on the builder.
            /// </summary>
            public CommandDescriptor Build()
            {
                var descriptor = new CommandDescriptor
                {
                    ImplementingType = _implementingType,
                    CommandName = _commandName
                };

                descriptor.SetDescriptions(_descriptions);

                return descriptor;
            }
        }
    }
}