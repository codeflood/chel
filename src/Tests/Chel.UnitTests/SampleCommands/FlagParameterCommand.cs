using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("command")]
    public class FlagParameterCommand : ICommand
    {
        [FlagParameter("p1")]
        [Description("The p1 parameter.")]
        public bool Param1 { get; set; }

        [FlagParameter("p2")]
        [Description("The p2 parameter.")]
        public bool Param2 { get; set; }

        public CommandResult Execute()
        {
            return new ValueResult($"p1: {Param1}, p2: {Param2}");
        }
    }
}