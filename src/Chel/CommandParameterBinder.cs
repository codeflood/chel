using System;
using System.Linq;
using System.Reflection;
using Chel.Abstractions;

namespace Chel
{
    public class CommandParameterBinder : ICommandParameterBinder
    {
        public ParameterBindResult Bind(ICommand instance, CommandInput input)
        {
            if(instance == null)
                throw new ArgumentNullException(nameof(instance));

            if(input == null)
                throw new ArgumentNullException(nameof(input));

            var result = new ParameterBindResult();

            // todo: performance optimization: create a map of the properties to parameters on the command.

            for(var num = 0; num < input.NumberedParameters.Count; num++)
            {
                BindNumberedParameter(instance, input, num, result);
            }

            return result;
        }

        private void BindNumberedParameter(ICommand instance, CommandInput input, int index, ParameterBindResult result)
        {
            var bound = false;

            // Parameter numbers are 1 indexed, not zero.
            var parameterNumber = index + 1;

            var properties = instance.GetType().GetProperties();
            foreach(var property in properties)
            {
                var attribute = property.GetCustomAttributes<NumberedParameterAttribute>().FirstOrDefault();
                if(attribute != null)
                {
                    if(attribute.Number == parameterNumber)
                    {
                        BindProperty(instance, property, input.NumberedParameters[index], result);
                        bound = true;
                    }
                }
            }

            if(!bound)
                result.AddError(string.Format(Texts.UnexpectedNumberedParameter, input.NumberedParameters[index]));
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