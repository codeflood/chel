using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions.Parsing;

namespace Chel.Abstractions.Types
{
	/// <summary>
	/// A single compound value.
	/// </summary>
	public class CompoundValue : List
    {
        public CompoundValue(IReadOnlyList<ICommandParameter> values)
            : base(values)
        {
            foreach(var value in values)
            {
                if(!(value is Literal) && !(value is VariableReference))
                    throw ExceptionFactory.CreateArgumentException(ApplicationTexts.CompoundValueOnlyConsistsLiteralsAndVariables, nameof(values));
            }
        }

        public override string ToString()
        {
            return string.Join(string.Empty, Values.Select(x => x.ToString()));
        }
    }
}