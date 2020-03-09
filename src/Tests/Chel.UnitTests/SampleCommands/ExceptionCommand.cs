using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("ex")]
    public class ExceptionCommand : ICommand
    {
        public CommandResult Execute()
        {
            throw new NotImplementedException();
        }
    }
}