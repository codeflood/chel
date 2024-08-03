using System.Text;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.UnitTests.SampleCommands
{
    [Command("☕")]
    public class AllParametersCommand : ICommand
    {
        [NumberedParameter(1, "num")]
        public string? NumberedParameter { get; set; }

        [NamedParameter("named", "value")]
        public string? NamedParameter { get; set; }

        [FlagParameter("flag")]
        public bool FlagParameter { get; set; }

        public CommandResult Execute()
        {
            var output = new StringBuilder();

            if(NumberedParameter != null)
                output.Append(NumberedParameter);

            if(NamedParameter != null)
                output.Append(NamedParameter);

            if(FlagParameter)
                output.Append("flag");

            return new ValueResult(new Literal(output.ToString()));
        }
    }
}