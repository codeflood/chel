using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chel.Abstractions;

namespace Chel
{
    public class CommandParameterBinder : ICommandParameterBinder
    {
        private const int UnhandledParameter = -1;

        public ParameterBindResult Bind(ICommand instance, CommandInput input)
        {
            if(instance == null)
                throw new ArgumentNullException(nameof(instance));

            if(input == null)
                throw new ArgumentNullException(nameof(input));

            var result = new ParameterBindResult();

            // todo: performance optimization: create a map of the properties to parameters on the command.

            var namedParameterInputKeys = input.NamedParameters.Keys.ToList();

            var properties = instance.GetType().GetProperties();
            var highestNumberedParameterHandled = UnhandledParameter;
            foreach(var property in properties)
            {
                var namedParameterBound = BindNamedParameter(instance, property, input, result);
                if(namedParameterBound != null)
                    namedParameterInputKeys.RemoveAll(x => StringComparer.OrdinalIgnoreCase.Equals(x, namedParameterBound));

                var parameterNumber = BindNumberedParameter(instance, property, input, result);
                if(parameterNumber > highestNumberedParameterHandled)
                    highestNumberedParameterHandled = parameterNumber;
            }

            if(namedParameterInputKeys.Count > 0)
            {
                foreach(var name in namedParameterInputKeys)
                {
                    result.AddError(string.Format(Texts.UnexpectedNamedParameter, name));
                }
            }

            if(input.NumberedParameters.Count > 0 && highestNumberedParameterHandled < input.NumberedParameters.Count)
            {
                for(var i = highestNumberedParameterHandled; i < input.NumberedParameters.Count; i++)
                {
                    result.AddError(string.Format(Texts.UnexpectedNumberedParameter, input.NumberedParameters[i]));
                }
            }

            return result;
        }

        private int BindNumberedParameter(ICommand instance, PropertyInfo property, CommandInput input, ParameterBindResult result)
        {
            var attribute = property.GetCustomAttributes<NumberedParameterAttribute>().FirstOrDefault();
            if(attribute != null)
            {
                if(!property.CanWrite)
                    throw new InvalidOperationException(string.Format(Texts.PropertyMissingSetter, property.Name, instance.GetType().FullName));

                if(input.NumberedParameters.Count >= attribute.Number)
                {
                    // Parameter numbers are 1 indexed, not zero.
                    var value = input.NumberedParameters[attribute.Number - 1];

                    BindProperty(instance, property, value, result);
                    return attribute.Number;
                }
                else
                {
                    var requiredAttribute = property.GetCustomAttributes<RequiredAttribute>().FirstOrDefault();
                    if(requiredAttribute != null)
                        result.AddError(string.Format(Texts.MissingRequiredNumberedParameter, attribute.PlaceholderText));
                }
            }

            return UnhandledParameter;
        }

        private string BindNamedParameter(ICommand instance, PropertyInfo property, CommandInput input, ParameterBindResult result)
        {
            var attribute = property.GetCustomAttributes<NamedParameterAttribute>().FirstOrDefault();
            if(attribute == null)
                return null;

            if(!property.CanWrite)
                throw new InvalidOperationException(string.Format(Texts.PropertyMissingSetter, property.Name, instance.GetType().FullName));

            if(input.NamedParameters.ContainsKey(attribute.Name))                
            {
                var value = input.NamedParameters[attribute.Name];
                BindProperty(instance, property, value, result);
                return attribute.Name;
            }
            else
            {
                var requiredAttribute = property.GetCustomAttributes<RequiredAttribute>().FirstOrDefault();
                if(requiredAttribute != null)
                    result.AddError(string.Format(Texts.MissingRequiredNamedParameter, attribute.Name));
            }

            return null;
        }

        private void BindProperty(ICommand instance, PropertyInfo property, string value, ParameterBindResult result)
        {
            if (property.PropertyType.IsAssignableFrom(value.GetType()))
                property.SetValue(instance, value);
            else
            {
                //result.AddError(string.Format(Texts.))

                /*
                // Find an appropriate type converter.
                TypeConverter converter = null;

                // First look for type converters on the property itself
                var propertyTypeConverter = property.GetCustomAttribute(typeof(TypeConverterAttribute));
                if (propertyTypeConverter != null)
                {
                    var converterType = Type.GetType((propertyTypeConverter as TypeConverterAttribute).ConverterTypeName, true, false);
                    converter = Activator.CreateInstance(converterType) as TypeConverter;
                }
                else
                    // Otherwise allow type descriptor to find a converter
                    converter = TypeDescriptor.GetConverter(property.PropertyType);

                if (converter != null)
                    property.SetValue(hostObject, converter.ConvertFrom(value));
                    */
            }
        }
    }
}