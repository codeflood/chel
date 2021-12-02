using System;
using System.Collections.Generic;
using System.Text;
using Chel.Abstractions.Parsing;

namespace Chel.Abstractions.Types
{
    /// <summary>
    /// A map of keys to values.
    /// </summary>
    public class Map : ChelType
    {
        private const int ElementPerLineLengthLimit = 15;

        /// <summary>
        /// Gets the entries of the map.
        /// </summary>
        public IReadOnlyDictionary<string, ICommandParameter> Entries { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="entries">The entries of the map.</param>
        public Map(IDictionary<string, ICommandParameter> entries)
        {
            if(entries != null)
                Entries = new Dictionary<string, ICommandParameter>(entries, StringComparer.OrdinalIgnoreCase);
            else
                Entries = new Dictionary<string, ICommandParameter>(StringComparer.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            
            var other = (Map)obj;
            
            if(Entries.Count != other.Entries.Count)
                return false;

            foreach(var key in Entries.Keys)
            {
                if(!other.Entries.ContainsKey(key))
                    return false;

                if(!Entries[key].Equals(other.Entries[key]))
                    return false;
            }

            return true;
        }
        
        public override int GetHashCode()
        {
            var hashCode = 23; // Start on a prime number

            unchecked
            {
                foreach(var entry in Entries)
                {
                    hashCode = hashCode * 3067 + entry.Key.GetHashCode(); // multiply by prime number
                    hashCode = hashCode * 3067 + entry.Value.GetHashCode(); // multiply by prime number
                }
            }

            return hashCode;
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append("{");
            var longValue = false;

            foreach(var entry in Entries)
            {
                var outputKey = entry.Key.ToString();
                var outputValue = entry.Value.ToString();

                if(outputKey.Length + outputValue.Length <= ElementPerLineLengthLimit)
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

                output.Append(outputKey);
                output.Append(": ");

                var mustWrap = MustWrap(outputValue);

                if(mustWrap)
                    output.Append("(");

                output.Append(outputValue);

                if(mustWrap)
                    output.Append(")");
            }

            if(longValue)
                output.Append(Environment.NewLine);
            else
                output.Append(" ");

            output.Append("}");

            return output.ToString();
        }

        private bool MustWrap(string value)
        {
            if(string.IsNullOrEmpty(value))
                return false;

            var startChar = value[0];

            if(startChar == Symbol.BlockStart ||
                startChar == Symbol.ListStart ||
                startChar == Symbol.MapStart
            )
                return false;

            foreach(var c in value)
            {
                if(char.IsWhiteSpace(c))
                    return true;
            }

            return false;
        }
    }
}