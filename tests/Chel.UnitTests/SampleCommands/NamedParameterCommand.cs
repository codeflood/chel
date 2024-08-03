using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.UnitTests.SampleCommands
{
    [Command("nam")]
    [Description("A sample command with named parameters.")]
    public class NamedParameterCommand : ICommand
    {
        [NamedParameter("param1", "value1")]
        [Description("The param1 parameter.")]
        public string? NamedParameter1 { get; set; }

        [NamedParameter("param2", "value2")]
        [Description("The param2 parameter.")]
        public string? NamedParameter2 { get; set; }

        public NamedParameterCommand()
        {
            NamedParameter1 = null;
            NamedParameter2 = null;
        }

        public CommandResult Execute()
        {
            var value = (NamedParameter1 ?? string.Empty) + " " + (NamedParameter2 ?? string.Empty);
            return new ValueResult(new Literal(value));
        }
    }
}