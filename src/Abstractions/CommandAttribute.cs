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
        /// Gets the description for the command.
        /// </summary>
        public string Description { get; private set; }
        
        public CommandAttribute(string commandName, string description)
        {
            if(commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if(string.IsNullOrEmpty(commandName))
                throw new InvalidCommandNameException(commandName);

            if(description == null)
                throw new ArgumentNullException(nameof(description));

            if(string.IsNullOrEmpty(description))
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(description)), nameof(description));
                
            CommandName = commandName;
            Description = description;
        }
    }
}