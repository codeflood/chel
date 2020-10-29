using System;

namespace Chel.Parsing
{
    internal class LiteralToken : Token
    {
        public string Value { get; }

        public LiteralToken(string value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }
    }
}
