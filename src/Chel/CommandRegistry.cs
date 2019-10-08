using System;
using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions;

namespace Chel
{
    /// <summary>
    /// The default command registry.
    /// </summary>
    public class CommandRegistry : ICommandRegistry
    {
        private readonly Type _commandInterfaceType = typeof(ICommand);
        private readonly INameValidator _nameValidator = null;
        private readonly Dictionary<string, CommandDescriptor> _registeredTypes = null;

        public CommandRegistry(INameValidator nameValidator)
        {
            if(nameValidator == null)
                throw new ArgumentNullException(nameof(nameValidator));

            _nameValidator = nameValidator;
            _registeredTypes = new Dictionary<string, CommandDescriptor>(StringComparer.OrdinalIgnoreCase);
        }

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
            var builder = new CommandDescriptor.Builder(type, attribute.CommandName, attribute.Description);
            return builder.Build();
        }

        public CommandDescriptor Resolve(string commandName)
        {
            if(commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if(_registeredTypes.ContainsKey(commandName))
                return _registeredTypes[commandName];

            return null;
        }

        public IEnumerable<CommandDescriptor> GetAllRegistrations()
        {
            return _registeredTypes.Values;
        }
    }
}