using System.Collections.Generic;

namespace Chel.Abstractions.Types
{
	/// <summary>
	/// A single compound value.
	/// </summary>
	public class CompoundValue : List
    {
        // todo: only accept Literals and variables.

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
        }
    }
}