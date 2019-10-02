using System;

namespace Chel.Abstractions.Results
{
    public class ValueResult : SuccessResult
    {
        public string Value { get; set; }

        public ValueResult(string value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}