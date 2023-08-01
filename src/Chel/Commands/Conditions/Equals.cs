using System;
using System.Threading;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands.Conditions
{
    [Command("eq")]
    [Description("Compare 2 values for equality")]
    public class Equals : ICommand
    {
        /// <summary>
        /// Gets the <see cref="IPhraseDictionary"/> used to resolve globalized phrases.
        /// </summary>
        protected IPhraseDictionary PhraseDictionary { get;}

        [NumberedParameter(1, "first")]
        [Description("The first value to check")]
        [Required]
        public ChelType FirstOperand { get; set; }

        [NumberedParameter(2, "second")]
        [Description("The second value to check")]
        [Required]
        public ChelType SecondOperand { get; set; }

        [FlagParameter("num")]
        [Description("Treat the values as numeric")]
        public bool IsNumeric { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="phraseDictionary">The <see cref="IPhraseDictionary"/> used to resolve globalized phrases.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="phraseDictionary"/> is null.</exception>
        public Equals(IPhraseDictionary phraseDictionary)
        {
            PhraseDictionary = phraseDictionary ?? throw new ArgumentNullException(nameof(phraseDictionary));
        }

        public CommandResult Execute()
        {
            if(FirstOperand == null)
            {
                // todo: Get culture from context. The command might be executed remotely and need a different culture.
                var message = PhraseDictionary.GetPhrase(Texts.PhraseKeys.NumberedParameterNotSet, Thread.CurrentThread.CurrentCulture.Name);
                return new FailureResult(string.Format(message, "1"));
            }

            if(SecondOperand == null)
            {
                // todo: Get culture from context. The command might be executed remotely and need a different culture.
                var message = PhraseDictionary.GetPhrase(Texts.PhraseKeys.NumberedParameterNotSet, Thread.CurrentThread.CurrentCulture.Name);
                return new FailureResult(string.Format(message, "2"));
            }

            if(IsNumeric)
                return CompareNumeric();

            return CompareNormal();
        }

        private CommandResult CompareNormal()
        {
            var value = FirstOperand.Equals(SecondOperand) ? "true" : "false";
            return new ValueResult(new Literal(value));
        }

        private CommandResult CompareNumeric()
        {
            var message = PhraseDictionary.GetPhrase(Texts.PhraseKeys.CouldNotParseNumber, Thread.CurrentThread.CurrentCulture.Name);

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
    }
}
