using System.Collections.Generic;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Sandbox.Commands
{
    [Command("random")]
    [Description("Generate a random number or select a random value from a list.")]
    public class Random : ICommand
    {
        private System.Random _random;

        [NamedParameter("min", "value")]
        [Description("The minimum value to generate.")]
        public int MinimumValue { get; set; } = 0;

        [NamedParameter("max", "value")]
        [Description("The maximum value to generate.")]
        public int MaximumValue { get; set; } = 100;

        [NamedParameter("options", "values")]
        [Description("The list of values to make a selection from.")]
        public IList<ChelType> Values { get; set; }

        public Random()
        {
            _random = new System.Random();
        }

        public CommandResult Execute()
        {
            ChelType output = null;

            if(Values.Count > 0)
                output = ExecuteRandomSelection();
            else
                output = ExecuteRandomNumber();

            return new ValueResult(output);
        }

        private ChelType ExecuteRandomSelection()
        {
            var num = _random.Next(0, Values.Count);
            return Values[num];
        }

        private ChelType ExecuteRandomNumber()
        {
            var num = _random.Next(MinimumValue, MaximumValue);
            return new Literal(num.ToString());
        }
    }
}
