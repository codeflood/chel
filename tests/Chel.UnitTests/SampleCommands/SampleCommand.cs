using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("sample")]
    [Description("description")]
    [Description("das description", "de")]
    public class SampleCommand : ICommand
    {
        public CommandResult Execute()
        {
            return new SuccessResult();
        }
    }
}