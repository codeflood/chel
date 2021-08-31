using System.Collections.Generic;
using Chel.Abstractions;
using Chel.Abstractions.Results;

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
        public IList<string> Values { get; set; }

        public Random()
        {
            _random = new System.Random();
        }

        public CommandResult Execute()
        {
            string output;

            if(Values.Count > 0)
                output = ExecuteRandomSelection();
            else
                output = ExecuteRandomNumber();

            return new ValueResult(output.ToString());
        }

        private string ExecuteRandomSelection()
        {
            var num = _random.Next(0, Values.Count);
            return Values[num];
        }

        private string ExecuteRandomNumber()
        {
            var num = _random.Next(MinimumValue, MaximumValue);
            return num.ToString();
        }
    }
}