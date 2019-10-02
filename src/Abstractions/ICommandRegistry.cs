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
        /// Resolve the <see cref="Type"/> which implements a specific command name.
        /// </summary>
        /// <param name="commandName">The name of the command to resolve.</param>
        Type Resolve(string commandName);

        /// <summary>
        /// Gets all the command type registrations.
        /// </summary>
        IEnumerable<Type> GetAllRegistrations();
    }
}