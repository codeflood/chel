using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Describes a command.
    /// </summary>
    public class CommandDescriptor
    {
        /// <summary>
        /// Gets the <see cref="Type"/> implementing the command.
        /// </summary>
        public Type ImplementingType { get; private set; }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string CommandName { get; private set; }

        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        public string Description { get; private set; }

        private CommandDescriptor()
        {
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

        /// <summary>
        /// Builds instances of <see cref="CommandDescriptor" />.
        /// </summary>
        public class Builder
        {
            /// <summary>
            /// Gets the <see cref="Type"/> implementing the command.
            /// </summary>
            public Type ImplementingType { get; set; }

            /// <summary>
            /// Gets the name of the command.
            /// </summary>
            public string CommandName { get; set; }

            /// <summary>
            /// Gets the description of the command.
            /// </summary>
            public string Description { get; set; }

            public Builder(Type implementingType, string commandName, string description)
            {
                if(implementingType == null)
                    throw new ArgumentNullException(nameof(implementingType));

                if(commandName == null)
                    throw new ArgumentNullException(nameof(commandName));

                if(commandName == string.Empty)
                    throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(commandName)), nameof(commandName));

                if(description == null)
                    throw new ArgumentNullException(nameof(description));

                if(description == string.Empty)
                    throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(description)), nameof(description));

                ImplementingType = implementingType;
                CommandName = commandName;
                Description = description;
            }

            public CommandDescriptor Build()
            {
                if(ImplementingType == null)
                    throw new InvalidOperationException(string.Format(Texts.ArgumentCannotBeNull, nameof(ImplementingType)));

                if(string.IsNullOrEmpty(CommandName))
                    throw new InvalidOperationException(string.Format(Texts.ArgumentCannotBeNullOrEmpty, nameof(CommandName)));

                if(string.IsNullOrEmpty(Description))
                    throw new InvalidOperationException(string.Format(Texts.ArgumentCannotBeNullOrEmpty, nameof(Description)));

                return new CommandDescriptor
                {
                    ImplementingType = ImplementingType,
                    CommandName = CommandName,
                    Description = Description
                };
            }
        }
    }
}