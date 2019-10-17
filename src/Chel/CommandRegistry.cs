using System;
using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions;

namespace Chel
{
    // <summary>
    /// The default implementation of the <see cref="ICommandRegistry" />.
    /// </summary>
    public class CommandRegistry : ICommandRegistry
    {
        private readonly Type _commandInterfaceType = typeof(ICommand);
        private readonly INameValidator _nameValidator = null;
        private readonly Dictionary<string, CommandDescriptor> _registeredTypes = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="nameValidator">A <see cref="INameValidator" /> used to validate the name of the command.</param>
        public CommandRegistry(INameValidator nameValidator)
        {
            if(nameValidator == null)
                throw new ArgumentNullException(nameof(nameValidator));

            _nameValidator = nameValidator;
            _registeredTypes = new Dictionary<string, CommandDescriptor>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Register a command.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> implementing the command.</param>
        public void Register(Type type)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            var implementsInterface = DoesImplementICommand(type);
            if(!implementsInterface)
                throw new ArgumentException(string.Format(Texts.ParameterDoesNotImplementICommand, type.FullName), nameof(type));

            var attribute = ExtractCommandAttribute(type);
            if(attribute == null)
                throw new ArgumentException(string.Format(Texts.ParameterIsNotAttributedWithCommandAttribute, type.FullName), nameof(type));

            var commandName = attribute.CommandName;

            var validCommandName = _nameValidator.IsValid(commandName);
            if(!validCommandName)
                throw new InvalidCommandNameException(commandName);

            var descriptor = DescribeCommandType(type, attribute);

            if(_registeredTypes.ContainsKey(commandName))
            {
                if(!_registeredTypes.ContainsValue(descriptor))
                    throw new CommandNameAlreadyUsedException(commandName, type, _registeredTypes[commandName].ImplementingType);
            }
            else
                _registeredTypes.Add(commandName, descriptor);
        }

        private bool DoesImplementICommand(Type type)
        {
            var interfaces = type.GetInterfaces();

            foreach(var i in interfaces)
            {
                if(i == _commandInterfaceType)
                    return true;
            }

            return false;
        }

        private CommandAttribute ExtractCommandAttribute(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(CommandAttribute), true);
            return (CommandAttribute)attributes.FirstOrDefault();
        }

        private CommandDescriptor DescribeCommandType(Type type, CommandAttribute attribute)
        {
            var builder = new CommandDescriptor.Builder(type, attribute.CommandName);

            var descriptionAttributes = ExtractDescriptionAttributes(type);
            foreach(var descriptionAttribute in descriptionAttributes)
            {
                builder.AddDescription(descriptionAttribute.Text, descriptionAttribute.CultureName);
            }

            return builder.Build();
        }

        private IEnumerable<DescriptionAttribute> ExtractDescriptionAttributes(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(DescriptionAttribute), true);
            return attributes.Select(x => (DescriptionAttribute)x);
        }

        /// <summary>
        /// Resolve a <see cref="CommandDescriptor" /> for a given command name.
        /// </summary>
        /// <param name="commandName">The name of the command to resolve.</param>
        public CommandDescriptor Resolve(string commandName)
        {
            if(commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if(_registeredTypes.ContainsKey(commandName))
                return _registeredTypes[commandName];

            return null;
        }

        /// <summary>
        /// Gets all the commands registered with the registry.
        /// </summary>
        public IEnumerable<CommandDescriptor> GetAllRegistrations()
        {
            return _registeredTypes.Values;
        }
    }
}