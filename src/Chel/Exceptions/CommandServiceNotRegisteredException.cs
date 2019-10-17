using System;

namespace Chel
{
    /// <summary>
    /// An <see cref="Exception" /> indicating a command service has not been registered.
    /// </summary>
    public class CommandServiceNotRegisteredException : Exception
    {
        /// <summary>
        /// Gets the <see cref="Type" /> of the service that was not registered.
        /// </summary>
        public Type CommandServiceType { get; private set; }
        
        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> of the service that was not registered.</param>
        public CommandServiceNotRegisteredException(Type type)
            : base(string.Format(Texts.CommandServiceNotRegistered, type?.FullName))
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            CommandServiceType = type;
        }
    }
}