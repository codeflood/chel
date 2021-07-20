using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A parameter value to be substituted for a variable value.
    /// </summary>
    public class VariableCommandParameter : CommandParameter
    {
        /// <summary>
        /// Gets the name of the variable to substitute.
        /// </summary>
        public string VariableName { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="variableName">The name of the variable to substitute.</param>
        public VariableCommandParameter(string variableName)
        {
            if(variableName == null)
                throw new ArgumentNullException(nameof(variableName));
            else if(string.IsNullOrEmpty(variableName))
                throw new ArgumentException(Texts.ArgumentCannotBeEmptyOrWhitespace, nameof(variableName));

            VariableName = variableName;
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || obj.GetType() != typeof(VariableCommandParameter))
                return false;
            
            var other = (VariableCommandParameter)obj;
            
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