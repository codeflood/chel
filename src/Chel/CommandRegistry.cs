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
        private readonly ICommandDescriptorGenerator _commandDescriptorGenerator = null;
        private readonly Dictionary<string, CommandDescriptor> _registeredTypes = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="nameValidator">A <see cref="INameValidator" /> used to validate the name of the command.</param>
        /// <param name="commandDescriptorGenerator">A <see cref="ICommandDescriptorGenerator" /> used to describe commands.</param>
        public CommandRegistry(INameValidator nameValidator, ICommandDescriptorGenerator commandDescriptorGenerator)
        {
            if(nameValidator == null)
                throw new ArgumentNullException(nameof(nameValidator));

            if(commandDescriptorGenerator == null)
                throw new ArgumentNullException(nameof(commandDescriptorGenerator));

            _nameValidator = nameValidator;
            _commandDescriptorGenerator = commandDescriptorGenerator;
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

            var descriptor = _commandDescriptorGenerator.DescribeCommand(type);
            if(descriptor == null)
                throw new InvalidOperationException(string.Format(Texts.DescriptorCouldNotBeGenerated, type.FullName));

            var validCommandName = _nameValidator.IsValid(descriptor.CommandName);
            if(!validCommandName)
                throw new InvalidCommandNameException(descriptor.CommandName);

            if(_registeredTypes.ContainsKey(descriptor.CommandName))
            {
                var existing = _registeredTypes[descriptor.CommandName];
                if(existing.ImplementingType != type)
                    throw new CommandNameAlreadyUsedException(descriptor.CommandName, type, _registeredTypes[descriptor.CommandName].ImplementingType);
            }
            else
                _registeredTypes.Add(descriptor.CommandName, descriptor);
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