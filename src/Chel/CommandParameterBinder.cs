using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
using Chel.Abstractions.Variables;
using Chel.Exceptions;

namespace Chel
{
    /// <summary>
    /// The default implementation of <see cref="ICommandParameterBinder" />.
    /// </summary>
    internal class CommandParameterBinder : ICommandParameterBinder
    {
        private ICommandRegistry _commandRegistry = null;
        private VariableCollection _variables = null;
        private IVariableReplacer _variableReplacer = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandRegistry">The <see cref="ICommandRegistry" /> used to resolve commands.</param>
        /// <param name="variableReplacer">The <see cref="IVariableReplacer" /> used to replace variables.</param>
        /// <param name="variables">The variables available for substitution.</param>
        public CommandParameterBinder(ICommandRegistry commandRegistry, IVariableReplacer variableReplacer, VariableCollection variables)
        {
            if(commandRegistry == null)
                throw new ArgumentNullException(nameof(commandRegistry));

            if(variableReplacer == null)
                throw new ArgumentNullException(nameof(variableReplacer));

            if(variables == null)
                throw new ArgumentNullException(nameof(variables));

            _commandRegistry = commandRegistry;
            _variableReplacer = variableReplacer;
            _variables = variables;
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

            var parameters = input.Parameters.ToList();

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

        private void BindFlagParameters(ICommand instance, CommandDescriptor descriptor, List<ChelType> parameters, ParameterBindResult result)
        {
            foreach(var describedParameter in descriptor.FlagParameters)
            {
                var markerIndex = FindParameterMarker(describedParameter.Name, parameters);

                if(markerIndex < 0)
                    continue;
                
                AssertWritableProperty(describedParameter, instance);
                BindProperty(instance, describedParameter.Property, describedParameter.Name, new Literal("True"), result);

                // Make sure there's no duplicates
                var repeatParameter = false;
                while(markerIndex >= 0)
                {
                    parameters.RemoveAt(markerIndex);
                    markerIndex = FindParameterMarker(describedParameter.Name, parameters);
                    if(markerIndex >= 0)
                        repeatParameter = true;
                }
                
                if(repeatParameter)
                    result.AddError(string.Format(Texts.CannotRepeatFlagParameter, describedParameter.Name));
            }
        }

        private void BindNamedParameters(ICommand instance, CommandDescriptor descriptor, List<ChelType> parameters, ParameterBindResult result)
        {
            foreach(var describedParameter in descriptor.NamedParameters.Values)
            {
                var markerIndex = FindParameterMarker(describedParameter.Name, parameters);

                if(markerIndex >= 0)
                {
                    if(markerIndex + 2 > parameters.Count)
                    {
                        result.AddError(string.Format(Texts.MissingValueForNamedParameter, describedParameter.Name));
                        parameters.RemoveAt(markerIndex);
                        continue;
                    }

                    var value = parameters[markerIndex + 1];
                    if(value is ParameterNameCommandParameter commandParameter)
                    {
                        // This is a parameter name, which cannot be a value.
                        result.AddError(string.Format(Texts.MissingValueForNamedParameter, describedParameter.Name));
                        parameters.RemoveAt(markerIndex);
                        continue;
                    }

                    AssertWritableProperty(describedParameter, instance);

                    try
                    {
                        BindProperty(instance, describedParameter.Property, describedParameter.Name, value, result);
                    }
                    catch(Exception ex)
                    {
                        result.AddError(string.Format(Texts.InvalidParameterValueForNamedParameter, value, describedParameter.Name, ex.Message));
                    }

                    // Make sure there's no duplicates
                    var repeatParameter = false;
                    while(markerIndex >= 0)
                    {
                        if(markerIndex + 2 <= parameters.Count)
                            parameters.RemoveAt(markerIndex + 1);

                        parameters.RemoveAt(markerIndex);
                        markerIndex = FindParameterMarker(describedParameter.Name, parameters);
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

        private void BindNumberedParameters(ICommand instance, CommandDescriptor descriptor, List<ChelType> parameters, ParameterBindResult result)
        {
            var boundParameterIndexes = new List<int>();

            foreach(var describedParameter in descriptor.NumberedParameters)
            {
                if(parameters.Count >= describedParameter.Number)
                {
                    // Parameter numbers are 1 indexed, not zero.
                    var value = parameters[describedParameter.Number - 1];

                    AssertWritableProperty(describedParameter, instance);

                    try
                    {
                        BindProperty(instance, describedParameter.Property, describedParameter.Number.ToString(), value, result);
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

        private void AssertNoNamedOrFlagParameters(List<ChelType> parameters, ParameterBindResult result)
        {
            for(var i = parameters.Count - 1; i >= 0; i--)
            {
                if(parameters[i] is ParameterNameCommandParameter commandParameter)
                {
                    // If the following parameter is a parameter name, we'll treat this one as a flag parameter.
                    if(parameters.Count > i + 1)
                    {
                        result.AddError(string.Format(Texts.UnknownNamedParameter, commandParameter.ParameterName));
                        parameters.RemoveAt(i + 1);
                    }
                    else
                        result.AddError(string.Format(Texts.UnknownFlagParameter, commandParameter.ParameterName));

                    parameters.RemoveAt(i);
                }
            }
        }

        private int FindParameterMarker(string marker, List<ChelType> parameters)
        {
            return parameters.FindIndex(x => {
                if(x is ParameterNameCommandParameter commandParameter)
                    return commandParameter.ParameterName.Equals(marker, StringComparison.OrdinalIgnoreCase);
                
                return false;
            });
        }

        private void AssertWritableProperty(ParameterDescriptor descriptor, object instance)
        {
            if(!descriptor.Property.CanWrite)
                throw new InvalidOperationException(string.Format(Texts.PropertyMissingSetter, descriptor.Property.Name, instance.GetType().FullName));
        }

        private void BindProperty(ICommand instance, PropertyInfo property, string parameterIdentifier, ChelType value, ParameterBindResult result)
        {
            var bindingValue = ReplaceVariables(value, result);
            if(bindingValue == null)
                return;

            if(property.PropertyType == typeof(ChelType))
            {
                property.SetValue(instance, bindingValue);
                return;
            }

            var listElementType = GetPropertyListType(property);

            if(bindingValue is List list)
            {
                BindListProperty(instance, property, listElementType, parameterIdentifier, list, result);
                return;
            }
            else if(listElementType != null)
            {
                result.AddError(string.Format(Texts.CannotBindNonListToListParameter, parameterIdentifier));
                return;
            }

            var convertedValue = ConvertPropertyValue(bindingValue, property.PropertyType, property);
            if(convertedValue == null)
                return;

            property.SetValue(instance, convertedValue);
        }

        private Type GetPropertyListType(PropertyInfo property)
        {
            if(property.PropertyType.IsGenericType)
            {
                var genericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();

                if(genericTypeDefinition == typeof(IEnumerable<>))
                    return property.PropertyType.GetGenericArguments()[0];
                else
                {
                    var interfaces = property.PropertyType.GetInterfaces();
                    foreach(var inf in interfaces)
                    {
                        if(!inf.IsGenericType)
                            continue;

                        if(typeof(IEnumerable<>) == inf.GetGenericTypeDefinition())
                        {
                            return inf.GetGenericArguments()[0];
                        }
                    }
                }
            }
            else if(property.PropertyType.HasElementType)
                return property.PropertyType.GetElementType();

            return null;
        }

        private void BindListProperty(ICommand instance, PropertyInfo property, Type listElementType, string parameterIdentifier, List value, ParameterBindResult result)
        {
            if(listElementType == null)
            {
                result.AddError(string.Format(Texts.CannotBindListToNonListParameter, parameterIdentifier));
                return;
            }
            
            var valuesType = typeof(List<>).MakeGenericType(listElementType);
            var values = Activator.CreateInstance(valuesType) as IList;
            foreach(var listValue in value.Values)
            {
                var bindingValue = ReplaceVariables(listValue, result);
                if(bindingValue == null)
                    continue;

                var convertedValue = ConvertPropertyValue(bindingValue, listElementType, property);
                
                values.Add(convertedValue);
            }

            if(property.PropertyType.IsArray)
            {
                var arrayValue = Array.CreateInstance(listElementType, values.Count);
                values.CopyTo(arrayValue, 0);
                property.SetValue(instance, arrayValue);
            }
            else
                property.SetValue(instance, values);
        }

        private ChelType ReplaceVariables(ChelType value, ParameterBindResult result)
        {
            try
            {
                return _variableReplacer.ReplaceVariables(_variables, value);
            }
            catch(UnsetVariableException ex)
            {
                result.AddError(ex.Message);
            }
            catch(ArgumentException ex)
            {
                result.AddError(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                result.AddError(ex.Message);
            }

            return null;
        }

        private object ConvertPropertyValue(object bindingValue, Type targetType, PropertyInfo property)
        {
            if(bindingValue is Literal literalBindingValue)
                bindingValue = literalBindingValue.Value;
            else if(bindingValue is CompoundValue compoundValueBindingValue)
                bindingValue = string.Join(string.Empty, compoundValueBindingValue.Values);

            if (targetType.IsAssignableFrom(bindingValue.GetType()))
                return bindingValue;
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
                    converter = TypeDescriptor.GetConverter(targetType);

                if (converter != null)
                    return converter.ConvertFrom(bindingValue);
            }

            return null;
        }
    }
}