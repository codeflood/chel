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
        public Type ImplementingType { get; }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string CommandName { get; }

        public CommandDescriptor(Type implementingType, string commandName)
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
    }
}