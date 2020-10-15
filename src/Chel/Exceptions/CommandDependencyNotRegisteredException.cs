using System;

namespace Chel
{
    /// <summary>
    /// An <see cref="Exception" /> indicating a command dependency has not been registered.
    /// </summary>
    public class CommandDependencyNotRegisteredException : Exception
    {
        /// <summary>
        /// Gets the <see cref="Type" /> of the service that was not registered.
        /// </summary>
        public Type CommandServiceType { get; }
        
        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> of the service that was not registered.</param>
        public CommandDependencyNotRegisteredException(Type type)
            : base(string.Format(Texts.CommandDependencyNotRegistered, type?.FullName))
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            CommandServiceType = type;
        }
    }
}