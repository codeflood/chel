using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chel.Abstractions.Types
{
    /// <summary>
    /// A list of values.
    /// </summary>
    public class List : ChelType
    {
        private const int ElementPerLineLengthLimit = 15;

        /// <summary>
        /// Gets the values of the list.
        /// </summary>
        public IReadOnlyList<ChelType> Values { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="values">The values of the list.</param>
        public List(IReadOnlyList<ChelType> values)
        {
            Values = values ?? Enumerable.Empty<ChelType>().ToList();
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || obj.GetType() != GetType())
                return false;
            
            var other = (List)obj;
            
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

        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append("[");
            var longValue = false;

            foreach(var value in Values)
            {
                var outputValue = value.ToString();

                if(outputValue.Length <= ElementPerLineLengthLimit)
                {
                    output.Append(" ");
                    longValue = false;
                }
                else
                {
                    output.Append(Environment.NewLine);
                    output.Append("  ");
                    longValue = true;
                }

                var containsWhitespace = ContainsWhitespace(outputValue);

                if(containsWhitespace)
                    output.Append("(");

                output.Append(outputValue);

                if(containsWhitespace)
                    output.Append(")");
            }

            if(longValue)
                output.Append(Environment.NewLine);
            else
                output.Append(" ");

            output.Append("]");

            return output.ToString();
        }

        private bool ContainsWhitespace(string value)
        {
            foreach(var c in value)
            {
                if(char.IsWhiteSpace(c))
                    return true;
            }

            return false;
        }
    }
}