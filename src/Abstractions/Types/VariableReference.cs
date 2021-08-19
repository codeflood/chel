using System;

namespace Chel.Abstractions.Types
{
    /// <summary>
    /// A reference to a variable.
    /// </summary>
    public class VariableReference : ChelType
    {
        /// <summary>
        /// Gets the name of the variable being referenced.
        /// </summary>
        public string VariableName { get; }

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
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || obj.GetType() != typeof(VariableReference))
                return false;
            
            var other = (VariableReference)obj;
            
            return VariableName.Equals(other.VariableName);
        }
        
        public override int GetHashCode()
        {
            return VariableName.GetHashCode();
        }

        public override string ToString()
        {
            return $"${VariableName}$";
        }
    }
}