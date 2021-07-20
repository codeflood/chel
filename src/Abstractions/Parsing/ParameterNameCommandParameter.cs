using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A parameter name used to mark a named parameter or flag parameter.
    /// </summary>
    public class ParameterNameCommandParameter : CommandParameter
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        public ParameterNameCommandParameter(string parameterName)
        {
            if(parameterName == null)
                throw new ArgumentNullException(nameof(parameterName));
            else if(string.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentException(Texts.ArgumentCannotBeEmptyOrWhitespace, nameof(parameterName));

            ParameterName = parameterName;
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || obj.GetType() != typeof(ParameterNameCommandParameter))
                return false;
            
            var other = (ParameterNameCommandParameter)obj;
            
            return ParameterName.Equals(other.ParameterName);
        }
        
        public override int GetHashCode()
        {
            return ParameterName.GetHashCode();
        }

        public override string ToString()
        {
            return $"-{ParameterName}";
        }
    }
}