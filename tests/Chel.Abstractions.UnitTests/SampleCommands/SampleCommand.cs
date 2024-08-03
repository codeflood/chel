using Chel.Abstractions.Results;

namespace Chel.Abstractions.UnitTests.SampleCommands
{
    [Command("sample")]
    [Description("description")]
    [Description("das description", "de")]
    public class SampleCommand : ICommand
    {
        [NumberedParameter(1, "param")]
        public string Parameter { get; set; }

        public CommandResult Execute()
        {
            return SuccessResult.Instance;
        }
    }
}