using System;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands.Conditions
{
    [Command("eq", "cond")]
    [Description("Compare 2 values for equality.")]
    public class Equals : ICommand
    {
        [NumberedParameter(1, "first")]
        [Description("The first value to check.")]
        [Required]
        public ChelType? FirstOperand { get; set; }

        [NumberedParameter(2, "second")]
        [Description("The second value to check.")]
        [Required]
        public ChelType? SecondOperand { get; set; }

        [FlagParameter("num")]
        [Description("Treat the values as numeric.")]
        public bool IsNumeric { get; set; }

        [FlagParameter("date")]
        [Description("Treat the values as dates.")]
        public bool IsDate { get; set; }

        [FlagParameter("guid")]
        [Description("Treat the values as GUIDs.")]
        public bool IsGuid { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterParser"/> used to parse typed values from the input.
        /// </summary>
        protected IParameterParser ParameterParser { get; set; }

        public Equals(IParameterParser parameterParser)
        {
            ParameterParser = parameterParser ?? throw new ArgumentNullException(nameof(parameterParser));
        }

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
            var value = FirstOperand!.Equals(SecondOperand) ? Constants.TrueLiteral : Constants.FalseLiteral;
            return new ValueResult(new Literal(value));
        }

        private CommandResult CompareNumeric()
        {
            var op1Result = ParameterParser.ParseDouble(FirstOperand!, "first");
            if(op1Result.HasError)
                return new FailureResult(op1Result.ErrorMessage ?? string.Empty);

            var op2Result = ParameterParser.ParseDouble(SecondOperand!, "second");
            if(op2Result.HasError)
                return new FailureResult(op2Result.ErrorMessage ?? string.Empty);

            var value = op1Result.Value == op2Result.Value ? Constants.TrueLiteral : Constants.FalseLiteral;
            return new ValueResult(new Literal(value));
        }

        private CommandResult CompareDates()
        {
            var op1Result = ParameterParser.ParseDateTime(FirstOperand!, "first");
            if(op1Result.HasError)
                return new FailureResult(op1Result.ErrorMessage ?? string.Empty);

            var op2Result = ParameterParser.ParseDateTime(SecondOperand!, "second");
            if(op2Result.HasError)
                return new FailureResult(op2Result.ErrorMessage ?? string.Empty);

            var value = op1Result.Value == op2Result.Value ? Constants.TrueLiteral : Constants.FalseLiteral;
            return new ValueResult(new Literal(value));
        }

        private CommandResult CompareGuids()
        {
            var op1Result = ParameterParser.ParseGuid(FirstOperand!, "first");
            if(op1Result.HasError)
                return new FailureResult(op1Result.ErrorMessage ?? string.Empty);

            var op2Result = ParameterParser.ParseGuid(SecondOperand!, "second");
            if(op2Result.HasError)
                return new FailureResult(op2Result.ErrorMessage ?? string.Empty);

            var value = op1Result.Value == op2Result.Value ? Constants.TrueLiteral : Constants.FalseLiteral;
            return new ValueResult(new Literal(value));
        }
    }
}
