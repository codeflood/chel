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
        /// Gets the optional name of the module the command belongs to.
        /// </summary>
        public string? ModuleName { get; private set; }

        /// <summary>
        /// Gets a <see cref="ExecutionTargetIdentifier"/> for the command.
        /// </summary>
        public ExecutionTargetIdentifier CommandIdentifier => new ExecutionTargetIdentifier(ModuleName, CommandName);

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        public CommandAttribute(string commandName)
            : this(commandName, null)
        {
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="moduleName">The name of the module the command belongs to.</param>
        public CommandAttribute(string commandName, string? moduleName)
        {
            if(commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if(string.IsNullOrEmpty(commandName))
                throw new InvalidNameException(commandName);
            
            if(moduleName != null && string.IsNullOrEmpty(moduleName))
                throw new InvalidNameException(moduleName);

            CommandName = commandName;
            ModuleName = moduleName;
        }
    }
}