using System.Collections.Generic;

namespace Chel.Abstractions.Types
{
	/// <summary>
	/// A single compound value.
	/// </summary>
	public class SingleValue : List
    {
        public SingleValue(Literal value)
            : base(value == null ? null : new List<ChelType> { value })
        {
        }

        public SingleValue(VariableReference value)
            : base(value == null ? null : new List<ChelType> { value })
        {
        }

        public SingleValue(IReadOnlyList<ChelType> values)
            : base(values)
        {
        }
    }
}