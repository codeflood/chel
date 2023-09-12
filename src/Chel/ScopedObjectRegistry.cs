using System;
using System.Collections.Generic;
using Chel.Abstractions;

namespace Chel
{
    public class ScopedObjectRegistry : IScopedObjectRegistry
    {
        private List<Type> _types = null;
        private Dictionary<Type, object> _instances = null;

        public ScopedObjectRegistry()
        {
            _types = new List<Type>();
            _instances = new Dictionary<Type, object>();
        }

        public void Register<T>()
        {
            var type = typeof(T);

            var constructor = type.GetConstructor(new Type[0]);
            if(constructor == null)
                throw new InvalidOperationException(ApplicationTextResolver.Instance.ResolveAndFormat(ApplicationTexts.TypeRequiresParameterlessConstructor, type.FullName));

            if(!_types.Contains(type))
                _types.Add(type);
        }

        public void RegisterInstance<T>(T instance)
        {
            if(instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = typeof(T);
            _instances.Add(type, instance);
        }

        public object Resolve(Type type)
        {
            object instance = null;

            if(_instances.ContainsKey(type))
                instance = _instances[type];
            else if(_types.Contains(type))
            {
                instance = Activator.CreateInstance(type);
                _instances.Add(type, instance);
            }
            
            return instance;
        }

        public IScopedObjectRegistry CreateScope()
        {
            var copy = new ScopedObjectRegistry();
            copy._types.AddRange(_types);
            return copy;
        }
    }
}