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
            culture = culture.Parent;

            while(culture != null && culture != culture.Parent)
            {
                if(_descriptions.ContainsKey(culture.Name))
                    return _descriptions[culture.Name];

                culture = culture.Parent;
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
            private Dictionary<string, string> _descriptions = new Dictionary<string, string>();

            /// <summary>
            /// Gets the <see cref="Type"/> implementing the command.
            /// </summary>
            public Type ImplementingType { get; set; }

            /// <summary>
            /// Gets the name of the command.
            /// </summary>
            public string CommandName { get; set; }

            public Builder(Type implementingType, string commandName)
            {
                if(implementingType == null)
                    throw new ArgumentNullException(nameof(implementingType));

                if(commandName == null)
                    throw new ArgumentNullException(nameof(commandName));

                if(commandName == string.Empty)
                    throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(commandName)), nameof(commandName));

                ImplementingType = implementingType;
                CommandName = commandName;
            }

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

            public CommandDescriptor Build()
            {
                if(ImplementingType == null)
                    throw new InvalidOperationException(string.Format(Texts.ArgumentCannotBeNull, nameof(ImplementingType)));

                if(string.IsNullOrEmpty(CommandName))
                    throw new InvalidOperationException(string.Format(Texts.ArgumentCannotBeNullOrEmpty, nameof(CommandName)));

                var descriptor = new CommandDescriptor
                {
                    ImplementingType = ImplementingType,
                    CommandName = CommandName
                };

                descriptor.SetDescriptions(_descriptions);

                return descriptor;
            }
        }
    }
}