using System;
using System.Collections.Generic;
using System.Text;
using Chel.Abstractions.Parsing;

namespace Chel.Abstractions.Types
{
    /// <summary>
    /// A reference to a variable.
    /// </summary>
    public class VariableReference : ICommandParameter
    {
        /// <summary>
        /// Gets the name of the variable being referenced.
        /// </summary>
        public string VariableName { get; }

        /// <summary>
        /// Gets the subreferences for the variable being referenced.
        /// </summary>
        public IReadOnlyList<string> SubReferences { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="variableName">The name of the variable being referenced.</param>
        /// <param name="subreferences">Subreferences for the variable being referenced.</param>
        public VariableReference(string variableName, IReadOnlyList<string> subreferences)
            : this(variableName)
        {
            if(subreferences != null)
                SubReferences = subreferences;
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="variableName">The name of the variable being referenced.</param>
        public VariableReference(string variableName)
        {
            if(variableName == null)
                throw new ArgumentNullException(nameof(variableName));
            else if(string.IsNullOrEmpty(variableName))
                throw new ArgumentException(Texts.ArgumentCannotBeEmptyOrWhitespace, nameof(variableName));

            VariableName = variableName;
            SubReferences = new List<string>();
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || obj.GetType() != typeof(VariableReference))
                return false;
            
            var other = (VariableReference)obj;

            if(SubReferences.Count != other.SubReferences.Count)
                return false;

            for(var i = 0; i < SubReferences.Count; i++)
            {
                if(SubReferences[i] != other.SubReferences[i])
                    return false;
            }
            
            return VariableName.Equals(other.VariableName);
        }
        
        public override int GetHashCode()
        {
            var hashCode = VariableName.GetHashCode();

            unchecked
            {
                foreach(var value in SubReferences)
                {
                    hashCode = hashCode * 3067 + value.GetHashCode(); // multiply by prime number
                }
            }

            return hashCode;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder(VariableName);
            
            foreach(var value in SubReferences)
            {
                buffer.Append(":");
                buffer.Append(value);
            }

            buffer.Insert(0, "$");
            buffer.Append("$");

            return buffer.ToString();
        }
    }
}
