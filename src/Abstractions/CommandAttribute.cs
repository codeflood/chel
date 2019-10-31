using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Provides the command name for the command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string CommandName { get; private set;}

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        public CommandAttribute(string commandName)
        {
            if(commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if(string.IsNullOrEmpty(commandName))
                throw new InvalidCommandNameException(commandName);

            CommandName = commandName;
        }
    }
}