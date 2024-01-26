using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands.Conditions
{
    [Command("gt")]
    [Description("Compare if value is greater than base.")]
    public class Greater : ICommand
    {
        [NumberedParameter(1, "base")]
        [Description("The base value for comparison.")]
        [Required]
        public ChelType Base { get; set; }

        [NumberedParameter(2, "value")]
        [Description("The value to compare to the base.")]
        [Required]
        public ChelType Value { get; set; }

        [FlagParameter("date")]
        [Description("Treat the values as dates.")]
        public bool IsDate { get; set; }
        
        public CommandResult Execute()
        {
            if(Base == null)
            {
                var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.NumberedParameterNotSet);
                return new FailureResult(string.Format(message, "1"));
            }

            if(Value == null)
            {
                var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.NumberedParameterNotSet);
                return new FailureResult(string.Format(message, "2"));
            }

            if(IsDate)
                return CompareDates();
            
            return CompareNumeric();
        }

        private CommandResult CompareNumeric()
        {
            var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.CouldNotParseNumber);

            var op1 = 0.0;
            if(Base is Literal literalFirstOperand)
            {
                if(!double.TryParse(literalFirstOperand.Value, out op1))
                    return new FailureResult(string.Format(message, Base));
            }
            else
                return new FailureResult(string.Format(message, "first"));

            var op2 = 0.0;
            if(Value is Literal literalSecondOperand)
            {
                if(!double.TryParse(literalSecondOperand.Value, out op2))
                    return new FailureResult(string.Format(message, Value));
            }
            else
                return new FailureResult(string.Format(message, "second"));

            var value = op2 > op1 ? "true" : "false";
            return new ValueResult(new Literal(value));
        }

        private CommandResult CompareDates()
        {
            var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.CouldNotParseDate);

            var op1 = DateTime.Now;
            if(Base is Literal literalFirstOperand)
            {
                if(!DateTime.TryParse(literalFirstOperand.Value, out op1))
                    return new FailureResult(string.Format(message, Base));
            }
            else
                return new FailureResult(string.Format(message, "first"));

            var op2 = DateTime.Now;
            if(Value is Literal literalSecondOperand)
            {
                if(!DateTime.TryParse(literalSecondOperand.Value, out op2))
                    return new FailureResult(string.Format(message, Value));
            }
            else
                return new FailureResult(string.Format(message, "second"));

            var value = op2 > op1 ? "true" : "false";
            return new ValueResult(new Literal(value));
        }
    }
}
