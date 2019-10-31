using System;
using Chel.Abstractions;
using Chel.Commands;

namespace Chel
{
    /// <summary>
    /// The Chel runtime and host API.
    /// </summary>
    public class Runtime
    {
        private INameValidator _nameValidator = null;
        private ICommandRegistry _commandRegistry = null;
        private ICommandServices _commandServices = null;
        private ICommandParameterBinder _parameterBinder = null;
        private IParser _parser = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public Runtime()
        {
            _nameValidator = new NameValidator();
            _commandRegistry = new CommandRegistry(_nameValidator);
            _commandServices = new CommandServices();
            _parameterBinder = new CommandParameterBinder();
            _parser = new Parser();

            _commandRegistry.Register(typeof(Help));
            _commandServices.Register(_commandRegistry);
        }

        /// <summary>
        /// Register a command type.
        /// </summary>
        /// <param name="commandType">The <see cref="Type" /> implementing the command.</param>
        public void RegisterCommandType(Type commandType)
        {
            if(commandType == null)
                throw new ArgumentNullException(nameof(commandType));

            _commandRegistry.Register(commandType);
        }

        /// <summary>
        /// Register a command service.
        /// </summary>
        /// <typeparam name="T">The type of the service being registered.</typeparam>
        /// <param name="service">The service instance to register.</param>
        public void RegisterCommandService<T>(T service)
        {
            if(service == null)
                throw new ArgumentNullException(nameof(service));

            _commandServices.Register<T>(service);
        }

        /// <summary>
        /// Creates a new <see cref="ISession" />.
        /// </summary>
        public ISession NewSession()
        {
            var factory = new CommandFactory(_commandRegistry, _commandServices);
            return new Session(_parser, factory, _parameterBinder);
        }
    }
}