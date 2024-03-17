using System;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands.Conditions
{
    [Command("gt", "cond")]
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

        /// <summary>
        /// Gets or sets the <see cref="IParameterParser"/> used to parse typed values from the input.
        /// </summary>
        protected IParameterParser ParameterParser { get; set; }

        public Greater(IParameterParser parameterParser)
        {
            ParameterParser = parameterParser ?? throw new ArgumentNullException(nameof(parameterParser));
        }
        
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
            var op1Result = ParameterParser.ParseDouble(Base, "base");
            if(op1Result.HasError)
                return new FailureResult(op1Result.ErrorMessage);

            var op2Result = ParameterParser.ParseDouble(Value, "value");
            if(op2Result.HasError)
                return new FailureResult(op2Result.ErrorMessage);

            var value = op2Result.Value > op1Result.Value ? Constants.TrueLiteral : Constants.FalseLiteral;
            return new ValueResult(new Literal(value));
        }

        private CommandResult CompareDates()
        {
            var op1Result = ParameterParser.ParseDateTime(Base, "base");
            if(op1Result.HasError)
                return new FailureResult(op1Result.ErrorMessage);

            var op2Result = ParameterParser.ParseDateTime(Value, "value");
            if(op2Result.HasError)
                return new FailureResult(op2Result.ErrorMessage);

            var value = op2Result.Value > op1Result.Value ? Constants.TrueLiteral : Constants.FalseLiteral;
            return new ValueResult(new Literal(value));
        }
    }
}
