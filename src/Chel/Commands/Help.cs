using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.Commands
{
    [Command("help")]
    public class Help : ICommand
    {
        private ICommandRegistry _commandRegistry;

        public Help(ICommandRegistry commandRegistry)
        {
            if(commandRegistry == null)
                throw new ArgumentNullException(nameof(commandRegistry));

            _commandRegistry = commandRegistry;
        }

        public CommandResult Execute()
        {
            return new Success();
        }
    }
}