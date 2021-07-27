using System.Collections.Generic;
using System.Linq;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A list command parameter.
    /// </summary>
    public class ListCommandParameter : CommandParameter
    {
        /// <summary>
        /// Gets the values of the list.
        /// </summary>
        public IReadOnlyList<CommandParameter> Values { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="values">The values of the list.</param>
        public ListCommandParameter(IReadOnlyList<CommandParameter> values)
        {
            Values = values ?? Enumerable.Empty<CommandParameter>().ToList();
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || obj.GetType() != typeof(ListCommandParameter))
                return false;
            
            var other = (ListCommandParameter)obj;
            
            if(Values.Count != other.Values.Count)
                return false;

            for(var i = 0; i < Values.Count; i++)
            {
                if(!Values[i].Equals(other.Values[i]))
                    return false;
            }

            return true;
        }
        
        public override int GetHashCode()
        {
            var hasCode = 0;
            foreach(var value in Values)
            {
                hasCode += value.GetHashCode();
            }

            return hasCode;
        }
    }
}