using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A parameter name used to mark a named parameter or flag parameter.
    /// </summary>
    public class ParameterNameCommandParameter : SourceCommandParameter
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        public ParameterNameCommandParameter(SourceLocation sourceLocation, string parameterName)
            : base(sourceLocation)
        {
            if(parameterName == null)
                throw new ArgumentNullException(nameof(parameterName));
            else if(string.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentException(Texts.ArgumentCannotBeEmptyOrWhitespace, nameof(parameterName));

            ParameterName = parameterName;
        }

        public override bool Equals(object obj)
        {
            if(!base.Equals(obj))
                return false;

            if (obj == null || obj.GetType() != typeof(ParameterNameCommandParameter))
                return false;
            
            var other = (ParameterNameCommandParameter)obj;
            
            return
                // SourceLocation is handled in the base class.
                ParameterName.Equals(other.ParameterName);
        }
        
        public override int GetHashCode()
        {
            var code = 29;
            unchecked
            {
                code += base.GetHashCode();
                code += ParameterName.GetHashCode();
            }

            return code;
        }

        public override string ToString()
        {
            return $"-{ParameterName}";
        }
    }
}