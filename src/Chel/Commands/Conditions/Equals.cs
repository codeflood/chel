using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands.Conditions
{
    [Command("eq")]
    [Description("Compare 2 values for equality.")]
    public class Equals : ICommand
    {
        [NumberedParameter(1, "first")]
        [Description("The first value to check.")]
        [Required]
        public ChelType FirstOperand { get; set; }

        [NumberedParameter(2, "second")]
        [Description("The second value to check.")]
        [Required]
        public ChelType SecondOperand { get; set; }

        [FlagParameter("num")]
        [Description("Treat the values as numeric.")]
        public bool IsNumeric { get; set; }

        [FlagParameter("date")]
        [Description("Treat the values as dates.")]
        public bool IsDate { get; set; }

        [FlagParameter("guid")]
        [Description("Treat the values as GUIDs.")]
        public bool IsGuid { get; set; }

        public CommandResult Execute()
        {
            if(FirstOperand == null)
            {
                var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.NumberedParameterNotSet);
                return new FailureResult(string.Format(message, "1"));
            }

            if(SecondOperand == null)
            {
                var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.NumberedParameterNotSet);
                return new FailureResult(string.Format(message, "2"));
            }

            var multipleFlagsMessage = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.CannotSetMultipleFlags);

            if(IsNumeric && IsDate)
                return new FailureResult(string.Format(multipleFlagsMessage, "num, date"));

            if(IsNumeric && IsGuid)
                return new FailureResult(string.Format(multipleFlagsMessage, "num, guid"));

            if(IsGuid && IsDate)
                return new FailureResult(string.Format(multipleFlagsMessage, "guid, date"));

            if(IsNumeric)
                return CompareNumeric();
            
            if(IsDate)
                return CompareDates();

            if(IsGuid)
                return CompareGuids();

            return CompareNormal();
        }

        private CommandResult CompareNormal()
        {
            var value = FirstOperand.Equals(SecondOperand) ? "true" : "false";
            return new ValueResult(new Literal(value));
        }

        private CommandResult CompareNumeric()
        {
            var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.CouldNotParseNumber);

            var op1 = 0.0;
            if(FirstOperand is Literal literalFirstOperand)
            {
                if(!double.TryParse(literalFirstOperand.Value, out op1))
                    return new FailureResult(string.Format(message, FirstOperand));
            }
            else
                return new FailureResult(string.Format(message, "first"));

            var op2 = 0.0;
            if(SecondOperand is Literal literalSecondOperand)
            {
                if(!double.TryParse(literalSecondOperand.Value, out op2))
                    return new FailureResult(string.Format(message, SecondOperand));
            }
            else
                return new FailureResult(string.Format(message, "second"));

            var value = op1 == op2 ? "true" : "false";
            return new ValueResult(new Literal(value));
        }

        private CommandResult CompareDates()
        {
            var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.CouldNotParseDate);

            var op1 = DateTime.Now;
            if(FirstOperand is Literal literalFirstOperand)
            {
                if(!DateTime.TryParse(literalFirstOperand.Value, out op1))
                    return new FailureResult(string.Format(message, FirstOperand));
            }
            else
                return new FailureResult(string.Format(message, "first"));

            var op2 = DateTime.Now;
            if(SecondOperand is Literal literalSecondOperand)
            {
                if(!DateTime.TryParse(literalSecondOperand.Value, out op2))
                    return new FailureResult(string.Format(message, SecondOperand));
            }
            else
                return new FailureResult(string.Format(message, "second"));

            var value = op1 == op2 ? "true" : "false";
            return new ValueResult(new Literal(value));
        }

        private CommandResult CompareGuids()
        {
            var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.CouldNotParseGuid);

            var op1 = Guid.Empty;
            if(FirstOperand is Literal literalFirstOperand)
            {
                if(!Guid.TryParse(literalFirstOperand.Value, out op1))
                    return new FailureResult(string.Format(message, FirstOperand));
            }
            else
                return new FailureResult(string.Format(message, "first"));

            var op2 = Guid.Empty;
            if(SecondOperand is Literal literalSecondOperand)
            {
                if(!Guid.TryParse(literalSecondOperand.Value, out op2))
                    return new FailureResult(string.Format(message, SecondOperand));
            }
            else
                return new FailureResult(string.Format(message, "second"));

            var value = op1 == op2 ? "true" : "false";
            return new ValueResult(new Literal(value));
        }
    }
}
