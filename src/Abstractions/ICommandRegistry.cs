using System;
using System.Collections.Generic;

namespace Chel.Abstractions
{
    /// <summary>
    /// Contains the commands available for execution.
    /// </summary>
    public interface ICommandRegistry
    {
        /// <summary>
        /// Register a type which implements <see cref="ICommand"/>.
        /// </summary>
        /// <param name="type">The type to register.</param>
        void Register(Type type);

        /// <summary>
        /// Resolve a command by it's name.
        /// </summary>
        /// <param name="commandIdentifier">The identifier of the command to resolve.</param>
        CommandDescriptor Resolve(ExecutionTargetIdentifier commandIdentifier);

        /// <summary>
        /// Gets all the command type registrations.
        /// </summary>
        IEnumerable<CommandDescriptor> GetAllRegistrations();
    }
}