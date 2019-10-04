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