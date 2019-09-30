using System;
using Chel.Abstractions;

namespace Chel
{
    public class Runtime
    {
        private INameValidator _nameValidator = null;
        private ICommandRegistry _commandRegistry = null;
        private IParser _parser = null;

        public Runtime()
        {
            _nameValidator = new NameValidator();
            _commandRegistry = new CommandRegistry(_nameValidator);
            _parser = new Parser();
        }

        public void RegisterCommandType(Type commandType)
        {
            if(commandType == null)
                throw new ArgumentNullException(nameof(commandType));

            _commandRegistry.Register(commandType);
        }

        public ISession NewSession()
        {
            var factory = new CommandFactory(_commandRegistry);
            return new Session(_parser, factory);
        }
    }
}