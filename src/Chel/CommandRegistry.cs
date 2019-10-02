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
        private readonly Dictionary<string, Type> _registeredTypes = null;

        public CommandRegistry(INameValidator nameValidator)
        {
            if(nameValidator == null)
                throw new ArgumentNullException(nameof(nameValidator));

            _nameValidator = nameValidator;
            _registeredTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        }

        public void Register(Type type)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            var implementsInterface = DoesImplementICommand(type);
            if(!implementsInterface)
                throw new ArgumentException($"{type.Name} does not implement ICommand.", nameof(type));

            var attribute = ExtractCommandAttribute(type);
            if(attribute == null)
                throw new ArgumentException($"{type.Name} is not attributed with CommandAttribute.", nameof(type));

            var commandName = attribute.CommandName;

            var validCommandName = _nameValidator.IsValid(commandName);
            if(!validCommandName)
                throw new ArgumentException($"'{commandName}' is not a valid command name.", nameof(type));

            if(_registeredTypes.ContainsKey(commandName))
            {
                if(!_registeredTypes.ContainsValue(type))
                    throw new ArgumentException($"Command name '{commandName}' on command {type.Name} is already used on command type {_registeredTypes[commandName].Name}.", nameof(type));
            }
            else
                _registeredTypes.Add(commandName, type);
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

        public Type Resolve(string commandName)
        {
            if(commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if(_registeredTypes.ContainsKey(commandName))
                return _registeredTypes[commandName];

            return null;
        }

        public IEnumerable<Type> GetAllRegistrations()
        {
            return _registeredTypes.Values;
        }
    }
}