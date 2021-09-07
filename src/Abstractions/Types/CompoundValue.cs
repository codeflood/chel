using System;
using System.Collections.Generic;
using System.Linq;

namespace Chel.Abstractions.Types
{
	/// <summary>
	/// A single compound value.
	/// </summary>
	public class CompoundValue : List
    {
        public CompoundValue(Literal value)
            : base(value == null ? null : new List<ChelType> { value })
        {
        }

        public CompoundValue(VariableReference value)
            : base(value == null ? null : new List<ChelType> { value })
        {
        }

        public CompoundValue(IReadOnlyList<ChelType> values)
            : base(values)
        {
            foreach(var value in values)
            {
                if(!(value is Literal) && !(value is VariableReference))
                    throw new ArgumentException(Texts.CompoundValueOnlyConsistsLiteralsAndVariables, nameof(values));
            }
        }

        public override string ToString()
        {
            return string.Join(string.Empty, Values.Select(x => x.ToString()));
        }
    }
}