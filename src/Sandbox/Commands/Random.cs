using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.Sandbox.Commands
{
    [Command("random")]
    [Description("Generate a random number.")]
    public class Random : ICommand
    {
        [NamedParameter("min", "value")]
        [Description("The minimum value to generate.")]
        public int MinimumValue { get; set; } = 0;

        [NamedParameter("max", "value")]
        [Description("The maximum value to generate.")]
        [Required]
        public int MaximumValue { get; set; }

        public CommandResult Execute()
        {
            var gen = new System.Random();
            var num = gen.Next(MinimumValue, MaximumValue);
            return new ValueResult(num.ToString());
        }
    }
}