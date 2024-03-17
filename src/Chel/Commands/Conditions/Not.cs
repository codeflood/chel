using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands.Conditions
{
    [Command("not", "cond")]
    [Description("Inverse a boolean value.")]
    public class Not : ICommand
    {
        [NumberedParameter(1, "value")]
        [Description("The value to inverse.")]
        [Required]
        public bool Value { get; set; }

        public CommandResult Execute()
        {
            var result = (Value ? false.ToString() : true.ToString()).ToLowerInvariant();
            return new ValueResult(new Literal(result));
        }
    }
}
