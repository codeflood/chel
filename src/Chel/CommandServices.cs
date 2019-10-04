using System;
using System.Collections.Generic;
using Chel.Abstractions;

namespace Chel
{
    public class CommandServices : ICommandServices
    {
        private Dictionary<Type, object> _serviceRegistrations = null;

        public CommandServices()
        {
            _serviceRegistrations = new Dictionary<Type, object>();
        }

        public void Register<T>(T service)
        {
            if(service == null)
                throw new ArgumentNullException(nameof(service));

            var serviceType = typeof(T);

            if(_serviceRegistrations.ContainsKey(serviceType))
                throw new InvalidOperationException(string.Format(Texts.ServiceTypeAlreadyRegistered, serviceType.FullName));

            _serviceRegistrations.Add(serviceType, service);
        }

        public T Resolve<T>()
        {
            var serviceType = typeof(T);
            return (T)Resolve(serviceType);
        }

        public object Resolve(Type type)
        {
            if(!_serviceRegistrations.ContainsKey(type))
                return null;

            return _serviceRegistrations[type];
        }
    }
}