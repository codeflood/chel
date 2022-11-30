using System;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A command parameter name which has been parsed from source.
    /// </summary>
    public class SourceNameCommandParameter : SourceCommandParameter
    {
        /// <summary>
        /// Gets the name of the command parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="sourceLocation">The location the name was parsed from.</param>
        /// <param name="name">The name of the command parameter.</param>
        public SourceNameCommandParameter(SourceLocation sourceLocation, string name)
            : base(sourceLocation)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));
            
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmptyOrWhitespace, nameof(name)), nameof(name));

            Name = name;
        }

        public override bool Equals(object obj)
        {
            if(!base.Equals(obj))
                return false;

            if (obj == null || obj.GetType() != typeof(SourceNameCommandParameter))
            {
                return false;
            }

            var other = (SourceNameCommandParameter)obj;
            
            return
                // SourceLocation is handled in the base class.
                Name.Equals(other.Name);
        }
        
        public override int GetHashCode()
        {
            var code = 27;
            unchecked
            {
                code += base.GetHashCode();
                code += Name.GetHashCode();
            }

            return code;
        }
    }
}