using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// A single command input to be executed.
    /// </summary>
    public class CommandInput
    {
        /// <summary>
        /// Gets the name of the command to execute.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandName">The name of the command to execute.</param>
        public CommandInput(string commandName)
        {
            if(commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            CommandName = commandName;
        }

        public override bool Equals(object obj)
        {
            if(obj is CommandInput)
            {
                var other = obj as CommandInput;

                return string.Compare(CommandName, other.CommandName, true) == 0;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return CommandName.ToLower().GetHashCode();
        }
    }
}