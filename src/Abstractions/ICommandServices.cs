using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Contains the services available for injection into command instances.
    /// </summary>
    public interface ICommandServices
    {
        /// <summary>
        /// Register a service instance.
        /// </summary>
        /// <typeparam name="T">The type to register the service under.</typeparam>
        void Register<T>(T service);

        /// <summary>
        /// Resolve a service by type.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        T Resolve<T>();

        /// <summary>
        /// Resolve a service by type.
        /// </summary>
        object Resolve(Type type);
    }
}