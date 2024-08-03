using System;
using System.Collections.Generic;
using Chel.Abstractions;

namespace Chel
{
    /// <summary>
    /// The default implementation of the <see cref="ICommandServices" />.
    /// </summary>
    public class CommandServices : ICommandServices
    {
        private Dictionary<Type, object> _serviceRegistrations = new();

        /// <summary>
        /// Register a command service.
        /// </summary>
        /// <typeparam name="T">The type of the command service.</typeparam>
        public void Register<T>(T service)
        {
            if(service == null)
                throw new ArgumentNullException(nameof(service));

            var serviceType = typeof(T);

            if(_serviceRegistrations.ContainsKey(serviceType))
                throw ExceptionFactory.CreateInvalidOperationException(ApplicationTexts.ServiceTypeAlreadyRegistered, serviceType.FullName);

            _serviceRegistrations.Add(serviceType, service);
        }

        /// <summary>
        /// Resolve a command service by its type.
        /// </summary>
        /// <typeparam name="T">The type of the command service.</typeparam>
        public T? Resolve<T>()
        {
            var serviceType = typeof(T);
            return (T?)Resolve(serviceType);
        }

        /// <summary>
        /// Resolve a command service by its type.
        /// </summary>
        /// <param name="type">The type of the command service.</param>
        public object? Resolve(Type type)
        {
            if(!_serviceRegistrations.ContainsKey(type))
                return null;

            return _serviceRegistrations[type];
        }
    }
}