using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    public class ParameterTypesCommand : ICommand
    {
        public string String { get; set; }

        public CommandResult Execute()
        {
            throw new NotImplementedException();
        }
    }
}