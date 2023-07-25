using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Contains a collection of scoped objects.
    /// </summary>
    public interface IScopedObjectRegistry
    {
        /// <summary>
        /// Register a type.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        void Register<T>();

        /// <summary>
        /// Register an instance of a type.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="instance">The instance to register for the type.</param>
        void RegisterInstance<T>(T instance);

        /// <summary>
        /// Resolve a scoped object.
        /// </summary>
        /// <param name="type">The type of the object to resolve.</param>
        /// <returns>An instance of the requested type.</returns>
        object Resolve(Type type);

        /// <summary>
        /// Create a new <see cref="IScopedObjectRegistry"/> initialised from the registrations of this instance.
        /// </summary>
        IScopedObjectRegistry CreateScope();
    }
}