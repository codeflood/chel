using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Chel.Abstractions;

namespace Chel
{
    /// <summary>
    /// The default implementation of <see cref="ICommandParameterBinder" />.
    /// </summary>
    public class CommandParameterBinder : ICommandParameterBinder
    {
        private ICommandRegistry _commandRegistry = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandRegistry">The <see cref="ICommandRegistry" /> used to resolve commands.</param>
        public CommandParameterBinder(ICommandRegistry commandRegistry)
        {
            if(commandRegistry == null)
                throw new ArgumentNullException(nameof(commandRegistry));

            _commandRegistry = commandRegistry;
        }

        public ParameterBindResult Bind(ICommand instance, CommandInput input)
        {
            if(instance == null)
                throw new ArgumentNullException(nameof(instance));

            if(input == null)
                throw new ArgumentNullException(nameof(input));

            var result = new ParameterBindResult();

            var descriptor = _commandRegistry.Resolve(input.CommandName);
            if(descriptor == null)
                throw new InvalidOperationException(string.Format(Texts.DescriptorForCommandCouldNotBeResolved, input.CommandName));

            var parameters = new List<string>(input.Parameters);

            BindFlagParameters(instance, descriptor, parameters, result);
            BindNamedParameters(instance, descriptor, parameters, result);

            AssertNoNamedOrFlagParameters(parameters, result);

            BindNumberedParameters(instance, descriptor, parameters, result);       

            // Anything left over was unexpected
            foreach(var parameter in parameters)
            {
                result.AddError(string.Format(Texts.UnexpectedNumberedParameter, parameter));
            }

            return result;
        }

        private void BindFlagParameters(ICommand instance, CommandDescriptor descriptor, List<string> parameters, ParameterBindResult result)
        {
            foreach(var describedParameter in descriptor.FlagParameters)
            {
                var flagParameterMarker = "-" + describedParameter.Name;
                var markerIndex = FindParameterMarker(flagParameterMarker, parameters);

                if(markerIndex < 0)
                    continue;
                
                AssertWritableProperty(describedParameter, instance);
                BindProperty(instance, describedParameter.Property, "True", result);

                // Make sure there's no duplicates
                var repeatParameter = false;
                while(markerIndex >= 0)
                {
                    parameters.RemoveAt(markerIndex);
                    markerIndex = FindParameterMarker(flagParameterMarker, parameters);
                    if(markerIndex >= 0)
                        repeatParameter = true;
                }
                
                if(repeatParameter)
                    result.AddError(string.Format(Texts.CannotRepeatFlagParameter, describedParameter.Name));
            }
        }

        private void BindNamedParameters(ICommand instance, CommandDescriptor descriptor, List<string> parameters, ParameterBindResult result)
        {
            foreach(var describedParameter in descriptor.NamedParameters.Values)
            {
                var namedParameterMarker = "-" + describedParameter.Name;
                var markerIndex = FindParameterMarker(namedParameterMarker, parameters);

                if(markerIndex >= 0)
                {
                    if(markerIndex + 2 > parameters.Count)
                    {
                        result.AddError(string.Format(Texts.MissingValueForNamedParameter, describedParameter.Name));
                        parameters.RemoveAt(markerIndex);
                        continue;
                    }

                    var value = parameters[markerIndex + 1];
                    if(value.StartsWith("-"))
                    {
                        // This is a name marker, which cannot be a value.
                        result.AddError(string.Format(Texts.MissingValueForNamedParameter, describedParameter.Name));
                        parameters.RemoveAt(markerIndex);
                        continue;
                    }

                    value = UnescapeValue(value);

                    AssertWritableProperty(describedParameter, instance);

                    try
                    {
                        BindProperty(instance, describedParameter.Property, value, result);
                    }
                    catch(Exception)
                    {
                        result.AddError(string.Format(Texts.InvalidParameterValueForNamedParameter, value, describedParameter.Name));
                    }

                    // Make sure there's no duplicates
                    var repeatParameter = false;
                    while(markerIndex >= 0)
                    {
                        if(markerIndex + 2 <= parameters.Count)
                            parameters.RemoveAt(markerIndex + 1);

                        parameters.RemoveAt(markerIndex);
                        markerIndex = FindParameterMarker(namedParameterMarker, parameters);
                        if(markerIndex >= 0)
                            repeatParameter = true;
                    }
                    
                    if(repeatParameter)
                        result.AddError(string.Format(Texts.CannotRepeatNamedParameter, describedParameter.Name));
                }
                else
                {
                    if(describedParameter.Required)
                        result.AddError(string.Format(Texts.MissingRequiredNamedParameter, describedParameter.Name));
                }
            }
        }

        private void BindNumberedParameters(ICommand instance, CommandDescriptor descriptor, List<string> parameters, ParameterBindResult result)
        {
            var boundParameterIndexes = new List<int>();

            foreach(var describedParameter in descriptor.NumberedParameters)
            {
                if(parameters.Count >= describedParameter.Number)
                {
                    // Parameter numbers are 1 indexed, not zero.
                    var value = parameters[describedParameter.Number - 1];
                    value = UnescapeValue(value);

                    AssertWritableProperty(describedParameter, instance);

                    try
                    {
                        BindProperty(instance, describedParameter.Property, value, result);
                    }
                    catch(Exception)
                    {
                        result.AddError(string.Format(Texts.InvalidParameterValueForNumberedParameter, value, describedParameter.PlaceholderText));
                    }

                    boundParameterIndexes.Add(describedParameter.Number - 1);
                }
                else
                {
                    if(describedParameter.Required)
                        result.AddError(string.Format(Texts.MissingRequiredNumberedParameter, describedParameter.PlaceholderText));
                }
            }

            boundParameterIndexes.Reverse();
            foreach(var index in boundParameterIndexes)
                parameters.RemoveAt(index);
        }

        private void AssertNoNamedOrFlagParameters(List<string> parameters, ParameterBindResult result)
        {
            for(var i = parameters.Count - 1; i >= 0; i--)
            {
                if(parameters[i].StartsWith("-"))
                {
                    // If the following parameter starts with a dash, we'll treat this one as a flag parameter.
                    
                    if(parameters.Count > i + 1)
                    {
                        result.AddError(string.Format(Texts.UnknownNamedParameter, parameters[i].Substring(1)));
                        parameters.RemoveAt(i + 1);
                    }
                    else
                        result.AddError(string.Format(Texts.UnknownFlagParameter, parameters[i].Substring(1)));

                    parameters.RemoveAt(i);
                }
            }
        }

        private int FindParameterMarker(string marker, List<string> parameters)
        {
            return parameters.FindIndex(x => x.Equals(marker, StringComparison.OrdinalIgnoreCase));
        }

        private void AssertWritableProperty(ParameterDescriptor descriptor, object instance)
        {
            if(!descriptor.Property.CanWrite)
                throw new InvalidOperationException(string.Format(Texts.PropertyMissingSetter, descriptor.Property.Name, instance.GetType().FullName));
        }

        private string UnescapeValue(string parameterValue)
        {
            if(parameterValue.StartsWith(@"\"))
                return parameterValue.Substring(1);

            return parameterValue;
        }

        private void BindProperty(ICommand instance, PropertyInfo property, string value, ParameterBindResult result)
        {
            if (property.PropertyType.IsAssignableFrom(value.GetType()))
                property.SetValue(instance, value);
            else
            {
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
                    property.SetValue(instance, converter.ConvertFrom(value));
            }
        }
    }
}