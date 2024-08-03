using System;
using System.Linq;
using System.Reflection;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;

namespace Chel
{
    /// <summary>
    /// The default implementation of the <see cref="ICommandFactory" />.
    /// </summary>
    public class CommandFactory : ICommandFactory
    {
        private ICommandRegistry _commandRegistry;
        private ICommandServices _commandServices;
        private IScopedObjectRegistry _sessionObjects;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandRegistry">The commands that have been registered.</param>
        /// <param name="commandServices">The command services that have been registered.</param>
        /// <param name="sessionObjects">Objects available to the session.</summary>
        public CommandFactory(ICommandRegistry commandRegistry, ICommandServices commandServices, IScopedObjectRegistry sessionObjects)
        {
            if(commandRegistry == null)
                throw new ArgumentNullException(nameof(commandRegistry));

            if(commandServices == null)
                throw new ArgumentNullException(nameof(commandServices));

            if(sessionObjects == null)
                throw new ArgumentNullException(nameof(sessionObjects));

            _commandRegistry = commandRegistry;
            _commandServices = commandServices;
            _sessionObjects = sessionObjects;
        }

        /// <summary>
        /// Create a new instance of a command.
        /// </summary>
        /// <param name="commandInput">The <see cref="CommandInput" /> containing the details of the command to instantiate.</param>
        public ICommand? Create(CommandInput commandInput)
        {
            if(commandInput == null)
                throw new ArgumentNullException(nameof(commandInput));

            var descriptor = _commandRegistry.Resolve(commandInput.CommandIdentifier);

            if(descriptor == null)
                return null;

            var type = descriptor.ImplementingType;

            var constructor = type.GetConstructors().FirstOrDefault();
            var parameters = constructor?.GetParameters();
            object? commandInstance = null;

            if(constructor == null || !parameters.Any())
                commandInstance = Activator.CreateInstance(type);
            else
            {
                var parameterValues = ResolveParameters(parameters ?? Array.Empty<ParameterInfo>());
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
                    value = _sessionObjects.Resolve(parameters[i].ParameterType);

                if(value == null)
                    throw new CommandDependencyNotRegisteredException(parameters[i].ParameterType);

                parameterValues[i] = value;
            }

            return parameterValues;
        }
    }
}