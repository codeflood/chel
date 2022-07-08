using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chel.Abstractions
{
    public class PropertyDescriptor
    {
        /// <summary>
        /// Gets the reflected <see cref="PropertyInfo"/> for the property.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets whether the type of the property is compatible with a list parameter.
        /// </summary>
        public bool IsTypeListCompatible { get; private set; } = false;

        /// <summary>
        /// Gets whether the type of the property is compatible with a map parameter.
        /// </summary>
        public bool IsTypeMapCompatible { get; private set; } = false;

        /// <summary>
        /// Gets the type of the generic value, or null if the type is not generic.
        /// </summary>
        public Type GenericValueType { get; private set; } = null;

        /// <summary>
        /// Gets the type of the generic key, or null if the type is not generic.
        /// </summary>
        public Type GenericKeyType { get; private set; } = null;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="propertyInfo">The property the descriptor is for.</param>
        public PropertyDescriptor(PropertyInfo property)
        {
            if(property == null)
                throw new ArgumentNullException(nameof(property));
            
            Property = property;
            PopulatePropertiesFromType(Property.PropertyType);
        }

        private void PopulatePropertiesFromType(Type type)
        {
            if(type.IsGenericType)
            {
                var populated = PopulatePropertiesFromGenericType(type);
                if(!populated)
                {
                    var interfaces = type.GetInterfaces();
                    foreach(var inf in interfaces)
                    {
                        populated = PopulatePropertiesFromGenericType(inf);
                        if(populated)
                            return;
                    }
                }
            }
            
            else if(type.HasElementType)
                PopulateListCompatibleType(type.GetElementType());
        }

        private bool PopulatePropertiesFromGenericType(Type type)
        {
            if(type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if(genericTypeDefinition == typeof(IEnumerable<>))
                {
                    PopulateListCompatibleType(type.GetGenericArguments()[0]);
                    return true;
                }
                else if(genericTypeDefinition == typeof(IDictionary<,>))
                {
                    PopulateMapCompatibleType(type.GetGenericArguments()[0], type.GetGenericArguments()[1]);
                    return true;
                }
            }

            return false;
        }

        private void PopulateListCompatibleType(Type genericValueType)
        {
            IsTypeListCompatible = true;
            GenericValueType = genericValueType;
        }

        private void PopulateMapCompatibleType(Type genericKeyType, Type genericValueType)
        {
            IsTypeMapCompatible = true;
            GenericKeyType = genericKeyType;
            GenericValueType = genericValueType;
        }
    }
}