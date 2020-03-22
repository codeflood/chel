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
                var requiredAttribute = property.GetCustomAttributes(typeof(RequiredAttribute), true).FirstOrDefault();
                var descriptions = ExtractDescriptions(property);

                var attribute = property.GetCustomAttributes(typeof(NumberedParameterAttribute), true).FirstOrDefault();
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

                attribute = property.GetCustomAttributes(typeof(NamedParameterAttribute), true).FirstOrDefault();
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
            }
        }


    }
}