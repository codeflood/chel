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

            var parameters = ExtractParameterValues(input.Parameters, result, input.SourceLocation);

            if(!result.Success)
                return result;

            BindFlagParameters(instance, descriptor, parameters, result);
            BindNamedParameters(instance, descriptor, parameters, result);

            AssertNoNamedOrFlagParameters(parameters, result);

            BindNumberedParameters(instance, descriptor, parameters, result);

            // Anything left over was unexpected
            foreach(var parameter in parameters)
            {
                var reportValue = parameter.ToString();
                if(parameter is SourceValueCommandParameter sourceValueCommandParameter)
                    reportValue = sourceValueCommandParameter.Value.ToString();

                result.AddError(string.Format(Texts.UnexpectedNumberedParameter, reportValue));
            }

            return result;
        }

        private List<SourceCommandParameter> ExtractParameterValues(IReadOnlyList<ICommandParameter> parameters, ParameterBindResult result, SourceLocation fallbackLocation)
        {
            var output = new List<SourceCommandParameter>();

            foreach(var parameter in parameters)
            {
                if(parameter is CommandInput commandInput)
                {
                    if(commandInput.SubstituteValue == null)
                        throw new InvalidOperationException(Texts.CannotBindCommandInputWithoutSubstituteValue);

                    output.Add(new SourceValueCommandParameter(commandInput.SourceLocation, commandInput.SubstituteValue));
                }
                else if(parameter is SourceValueCommandParameter valueParameter)
                {
                    var value = valueParameter.Value;

                    // Don't use an 'is' check because CompoundValue is also a list.
                    if(value.GetType() == typeof(List))
                    {
                        var list = value as List;
                        var values = ExtractParameterValues(list.Values, result, valueParameter.SourceLocation);
                        output.Add(new SourceValueCommandParameter(valueParameter.SourceLocation, new List(values)));

                    }
                    else if(value is VariableReference variableReference && variableReference.VariableName[0] == Symbol.Expansion)
                    {
                        var variableName = variableReference.VariableName.Substring(1);
                        var updatedVariableReference = new VariableReference(variableName, variableReference.SubReferences);

                        var extractedVariable = _variableReplacer.ReplaceVariables(_variables, updatedVariableReference);

                        if(extractedVariable is Map extractedMap)
                        {
                            foreach(var entry in extractedMap.Entries)
                            {
                                output.Add(new ParameterNameCommandParameter(valueParameter.SourceLocation, entry.Key));
                                
                                var values = ExtractParameterValues(new[] { entry.Value }, result, valueParameter.SourceLocation);
                                // todo: add test for when map value is not a SourceValueCommandParameter
                                output.AddRange(values.Select(x => new SourceValueCommandParameter(valueParameter.SourceLocation, ((SourceValueCommandParameter)x).Value)));
                            }
                        }
                        else
                        {
                            result.AddError(string.Format(Texts.VariableIsNotMap, variableName));
                        }
                    }
                    else if(value is ChelType plainValue)
                        output.Add(new SourceValueCommandParameter(valueParameter.SourceLocation, plainValue));
                    
                }
                else if(parameter is SourceCommandParameter sourceParameter)
                {
                    output.Add(sourceParameter);
                }
                else if(parameter is ChelType plainValue)
                    output.Add(new SourceValueCommandParameter(fallbackLocation, plainValue));
            }

            //return output.Select(x => x as ICommandParameter).ToList();
            return output;
        }

        private void BindFlagParameters(ICommand instance, CommandDescriptor descriptor, List<SourceCommandParameter> parameters, ParameterBindResult result)
        {
            foreach(var describedParameter in descriptor.FlagParameters)
            {
                var markerIndex = FindParameterMarker(describedParameter.Name, parameters);

                if(markerIndex < 0)
                    continue;
                
                AssertWritableProperty(describedParameter, instance);
                BindProperty(instance, describedParameter.Property, describedParameter.Name, new SourceValueCommandParameter(parameters[markerIndex].SourceLocation, new Literal("True")), result);

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

        private void BindNamedParameters(ICommand instance, CommandDescriptor descriptor, List<SourceCommandParameter> parameters, ParameterBindResult result)
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
                        // todo: add test for incorrect type
                        if(!(value is SourceValueCommandParameter valueCommandParameter))
                            throw new InvalidOperationException("Unexpected parameter type");

                        BindProperty(instance, describedParameter.Property, describedParameter.Name, valueCommandParameter, result);
                    }
                    catch(Exception ex)
                    {
                        var reportValue = value.ToString();
                        if(value is SourceValueCommandParameter valueCommandParameter)
                            reportValue = valueCommandParameter.Value.ToString();

                        result.AddError(string.Format(Texts.InvalidParameterValueForNamedParameter, reportValue, describedParameter.Name, ex.Message));
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

        private void BindNumberedParameters(ICommand instance, CommandDescriptor descriptor, List<SourceCommandParameter> parameters, ParameterBindResult result)
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
                        // todo: add test for incorrect type
                        if(!(value is SourceValueCommandParameter valueCommandParameter))
                            throw new InvalidOperationException("Unexpected parameter type");

                        BindProperty(instance, describedParameter.Property, describedParameter.Number.ToString(), valueCommandParameter, result);
                    }
                    catch(Exception ex)
                    {
                        var reportValue = value.ToString();
                        if(value is SourceValueCommandParameter valueCommandParameter)
                            reportValue = valueCommandParameter.Value.ToString();

                        result.AddError(string.Format(Texts.InvalidParameterValueForNumberedParameter, reportValue, describedParameter.PlaceholderText, ex.Message));
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

        private void AssertNoNamedOrFlagParameters(List<SourceCommandParameter> parameters, ParameterBindResult result)
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

        private int FindParameterMarker(string marker, List<SourceCommandParameter> parameters)
        {
            return parameters.FindIndex(x => {
                if(x is ParameterNameCommandParameter commandParameter)
                    return commandParameter.ParameterName.Equals(marker, StringComparison.OrdinalIgnoreCase);
                
                return false;
            });
        }

        private void AssertWritableProperty(ParameterDescriptor descriptor, object instance)
        {
            if(!descriptor.Property.Property.CanWrite)
                throw new InvalidOperationException(string.Format(Texts.PropertyMissingSetter, descriptor.Property.Property.Name, instance.GetType().FullName));
        }

        private void BindProperty(ICommand instance, Abstractions.PropertyDescriptor propertyDescriptor, string parameterIdentifier, /*ChelType*/ SourceValueCommandParameter value, ParameterBindResult result)
        {
            var bindingValue = ReplaceVariables(value.Value, result);
            if(bindingValue == null)
                return;

            if(propertyDescriptor.Property.PropertyType == typeof(ChelType))
            {
                propertyDescriptor.Property.SetValue(instance, bindingValue);
                return;
            }

            if(bindingValue is List list)
            {
                BindListProperty(instance, propertyDescriptor.Property, propertyDescriptor.GenericValueType, parameterIdentifier, list, result);
                return;
            }
            else if(bindingValue is Map map)
            {
                BindDictionaryProperty(instance, propertyDescriptor.Property, propertyDescriptor.GenericKeyType, propertyDescriptor.GenericValueType, parameterIdentifier, map, result);
                return;
            }
            else if(propertyDescriptor.IsTypeListCompatible)
                result.AddError(string.Format(Texts.CannotBindNonListToListParameter, parameterIdentifier));
            else if(propertyDescriptor.IsTypeMapCompatible)
                result.AddError(string.Format(Texts.CannotBindNonMapToMapParameter, parameterIdentifier));

            var convertedValue = ConvertPropertyValue(bindingValue, propertyDescriptor.Property.PropertyType, propertyDescriptor.Property);
            if(convertedValue == null)
                return;

            propertyDescriptor.Property.SetValue(instance, convertedValue);
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
                var convertedValue = GetValue(listValue, Texts.ListValuesMustBeChelType, listElementType, property, result);
                if(convertedValue != null)
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

        private void BindDictionaryProperty(ICommand instance, PropertyInfo property, Type keyType, Type valueType, string parameterIdentifier, Map value, ParameterBindResult result)
        {
            if(keyType == null || valueType == null)
            {
                result.AddError(string.Format(Texts.CannotBindMapToNonMapParameter, parameterIdentifier));
                return;
            }

            if(keyType != typeof(string))
            {
                result.AddError(string.Format(Texts.KeyTypeMustBeString, property.DeclaringType.FullName + "." + property.Name));
                return;
            }

            var elementType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            var elements = Activator.CreateInstance(elementType) as IDictionary;
            foreach(var entry in value.Entries)
            {
                var convertedValue = GetValue(entry.Value, Texts.MapValuesMustBeChelType, valueType, property, result);
                if(convertedValue != null)
                    elements.Add(entry.Key, convertedValue);
            }

            property.SetValue(instance, elements);
        }

        private object GetValue(ICommandParameter value, string errorText, Type targetType, PropertyInfo property, ParameterBindResult result)
        {
            var wipValue = value;

            if(wipValue is SourceValueCommandParameter sourceValueCommandParameter)
                wipValue = sourceValueCommandParameter.Value;

            if(!wipValue.GetType().IsSubclassOf(typeof(ChelType)))
                throw new InvalidOperationException(errorText);

            var bindingValue = ReplaceVariables(wipValue as ChelType, result);
            if(bindingValue == null)
                return null;

            return ConvertPropertyValue(bindingValue, targetType, property);
        }

        private ICommandParameter ReplaceVariables(ICommandParameter value, ParameterBindResult result)
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