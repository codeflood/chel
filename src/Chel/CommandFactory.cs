using System;
using Chel.Abstractions;

namespace Chel
{
    public class CommandFactory : ICommandFactory
    {
        private ICommandRegistry _commandRegistry = null;

        public CommandFactory(ICommandRegistry commandRegistry)
        {
            if(commandRegistry == null)
                throw new ArgumentNullException(nameof(commandRegistry));

            _commandRegistry = commandRegistry;
        }

        public ICommand Create(CommandInput commandInput)
        {
            if(commandInput == null)
                throw new ArgumentNullException(nameof(commandInput));

            var type = _commandRegistry.Resolve(commandInput.CommandName);

            if(type == null)
                return null;

            var instance = Activator.CreateInstance(type);
            return instance as ICommand;
        }
    }
}