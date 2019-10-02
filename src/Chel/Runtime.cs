using System;
using Chel.Abstractions;
using Chel.Commands;

namespace Chel
{
    public class Runtime
    {
        private INameValidator _nameValidator = null;
        private ICommandRegistry _commandRegistry = null;
        private ICommandServices _commandServices = null;
        private IParser _parser = null;

        public Runtime()
        {
            _nameValidator = new NameValidator();
            _commandRegistry = new CommandRegistry(_nameValidator);
            _commandServices = new CommandServices();
            _parser = new Parser();

            _commandRegistry.Register(typeof(Help));
            _commandServices.Register(_commandRegistry);
        }

        public void RegisterCommandType(Type commandType)
        {
            if(commandType == null)
                throw new ArgumentNullException(nameof(commandType));

            _commandRegistry.Register(commandType);
        }

        public void RegisterCommandService<T>(T service)
        {
            if(service == null)
                throw new ArgumentNullException(nameof(service));

            _commandServices.Register<T>(service);
        }

        public ISession NewSession()
        {
            var factory = new CommandFactory(_commandRegistry, _commandServices);
            return new Session(_parser, factory);
        }
    }
}