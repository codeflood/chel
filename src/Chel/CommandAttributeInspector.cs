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
            var parameterDescriptors = ExtractParameters(commandType);

            return new CommandDescriptor(
                commandAttribute.CommandName,
                commandType,
                descriptions,
                new List<ParameterDescriptor>(parameterDescriptors)
            );
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

        private IEnumerable<ParameterDescriptor> ExtractParameters(Type type)
        {
            var properties = type.GetProperties();
            foreach(var property in properties)
            {
                var attribute = property.GetCustomAttributes(typeof(NumberedParameterAttribute), true).FirstOrDefault();
                if(attribute != null)
                {
                    var numberedParameterAttribute = attribute as NumberedParameterAttribute;
                    var descriptions = ExtractDescriptions(property);

                    yield return new NumberedParameterDescriptor(
                        numberedParameterAttribute.Number,
                        numberedParameterAttribute.PlaceholderText,
                        property,
                        descriptions
                    );
                }
            }
        }
    }
}