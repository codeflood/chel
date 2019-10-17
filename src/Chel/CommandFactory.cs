using System;
using System.Linq;
using System.Reflection;
using Chel.Abstractions;

namespace Chel
{
    /// <summary>
    /// The default implementation of the <see cref="ICommandFactory" />.
    /// </summary>
    public class CommandFactory : ICommandFactory
    {
        private ICommandRegistry _commandRegistry = null;
        private ICommandServices _commandServices = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandRegistry">The commands that have been registered.</param>
        /// <param name="commandServices">The command services that have been registered.</param>
        public CommandFactory(ICommandRegistry commandRegistry, ICommandServices commandServices)
        {
            if(commandRegistry == null)
                throw new ArgumentNullException(nameof(commandRegistry));

            if(commandServices == null)
                throw new ArgumentNullException(nameof(commandServices));

            _commandRegistry = commandRegistry;
            _commandServices = commandServices;
        }

        /// <summary>
        /// Create a new instance of a command.
        /// </summary>
        /// <param name="commandInput">The <see cref="CommandInput" /> containing the details of the command to instantiate.</param>
        public ICommand Create(CommandInput commandInput)
        {
            if(commandInput == null)
                throw new ArgumentNullException(nameof(commandInput));

            var descriptor = _commandRegistry.Resolve(commandInput.CommandName);

            if(descriptor == null)
                return null;

            var type = descriptor.ImplementingType;

            var constructor = type.GetConstructors().FirstOrDefault();
            var parameters = constructor?.GetParameters();
            object commandInstance = null;

            if(constructor == null || !parameters.Any())
                commandInstance = Activator.CreateInstance(type);
            else
            {
                var parameterValues = ResolveParameters(parameters);
                commandInstance = Activator.CreateInstance(type, parameterValues);
            }
            
            return commandInstance as ICommand;
        }

        private object[] ResolveParameters(ParameterInfo[] parameters)
        {
            var parameterValues = new object[parameters.Length];
            for(var i = 0; i < parameters.Length; i++)
            {
                var value = _commandServices.Resolve(parameters[i].ParameterType);
                if(value == null)
                    throw new CommandServiceNotRegisteredException(parameters[i].ParameterType);

                parameterValues[i] = value;
            }

            return parameterValues;
        }
    }
}