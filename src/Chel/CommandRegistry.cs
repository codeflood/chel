using System;
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
        private readonly INameValidator _nameValidator;

        public CommandRegistry(INameValidator nameValidator)
        {
            if(nameValidator == null)
                throw new ArgumentNullException(nameof(nameValidator));

            _nameValidator = nameValidator;
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

            var validCommandName = _nameValidator.IsValid(attribute.CommandName);
            if(!validCommandName)
                throw new ArgumentException($"'{attribute.CommandName}' is not a valid command name.", nameof(type));
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
            throw new NotImplementedException();
        }
    }
}