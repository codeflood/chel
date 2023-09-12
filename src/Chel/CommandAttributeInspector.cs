using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chel.Abstractions;
using Chel.Exceptions;

namespace Chel
{
    /// <summary>
    /// A <see cref="ICommandDescriptorGenerator"/> which describes commands based on attributes applied to the command members.
    /// </summary>
    public class CommandAttributeInspector : ICommandDescriptorGenerator
    {
        public CommandDescriptor DescribeCommand(Type commandType)
        {
            if(commandType == null)
                throw new ArgumentNullException(nameof(commandType));

            var commandAttribute = ExtractCommandAttribute(commandType);
            if(commandAttribute == null)
                throw new TypeNotACommandException(commandType);

            var descriptions = ExtractDescriptions(commandType);

            var builder = new CommandDescriptor.Builder(commandAttribute.CommandName, commandType, descriptions);

            AddParameters(commandType, builder);
            
            return builder.Build();
        }

        private CommandAttribute ExtractCommandAttribute(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(CommandAttribute), true);
            return (CommandAttribute)attributes.FirstOrDefault();
        }

        private IEnumerable<DescriptionAttribute> ExtractDescriptionAttributes(MemberInfo member)
        {
            var attributes = member.GetCustomAttributes(typeof(DescriptionAttribute), true);
            return attributes.Select(x => (DescriptionAttribute)x);
        }

        private ITextResolver ExtractDescriptions(MemberInfo member)
        {
            var descriptions = new LocalisedTexts();
            var descriptionAttributes = ExtractDescriptionAttributes(member);

            foreach(var descriptionAttribute in descriptionAttributes)
            {
                descriptions.AddText(descriptionAttribute.Text, descriptionAttribute.CultureName);
            }

            return descriptions;
        }

        private void AddParameters(Type type, CommandDescriptor.Builder builder)
        {
            var properties = type.GetProperties();
            foreach(var property in properties)
            {
                var requiredAttribute = property.GetCustomAttribute(typeof(RequiredAttribute), true);
                var descriptions = ExtractDescriptions(property);

                var attribute = property.GetCustomAttribute(typeof(NumberedParameterAttribute), true);
                if(attribute != null)
                {
                    var numberedParameterAttribute = attribute as NumberedParameterAttribute;
                    var descriptor = new NumberedParameterDescriptor(
                        numberedParameterAttribute.Number,
                        numberedParameterAttribute.PlaceholderText,
                        property,
                        descriptions,
                        requiredAttribute != null
                    );
                    builder.AddNumberedParameter(descriptor);
                }

                attribute = property.GetCustomAttribute(typeof(NamedParameterAttribute), true);
                if(attribute != null)
                {
                    var namedParameterAttribute = attribute as NamedParameterAttribute;
                    var descriptor = new NamedParameterDescriptor(
                        namedParameterAttribute.Name,
                        namedParameterAttribute.ValuePlaceholderText,
                        property,
                        descriptions,
                        requiredAttribute != null
                    );
                    builder.AddNamedParameter(descriptor);
                }

                attribute = property.GetCustomAttribute(typeof(FlagParameterAttribute), true);
                if(attribute != null)
                {
                    if(requiredAttribute != null)
                    {
                        var typeName = type.FullName;
                        var propertyPath = $"{typeName}.{property.Name}";
                        throw new InvalidParameterDefinitionException(
                            property,
                            ApplicationTextResolver.Instance.ResolveAndFormat(ApplicationTexts.FlagParametersCannotBeRequired, propertyPath)
                        );
                    }

                    var flagParameterAttribute = attribute as FlagParameterAttribute;
                    var descriptor = new FlagParameterDescriptor(
                        flagParameterAttribute.Name,
                        property,
                        descriptions,
                        requiredAttribute != null
                    );
                    builder.AddFlagParameter(descriptor);
                }
            }
        }
    }
}