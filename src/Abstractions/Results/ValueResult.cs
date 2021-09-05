using System;
using Chel.Abstractions.Types;

namespace Chel.Abstractions.Results
{
    public class ValueResult : SuccessResult
    {
        public ChelType Value { get; set; }

        public ValueResult(ChelType value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}