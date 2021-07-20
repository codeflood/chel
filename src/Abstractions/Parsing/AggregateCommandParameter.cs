using System.Collections.Generic;
using System.Linq;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// A command parameter which constitutes of a collection of command parameters.
    /// </summary>
    public class AggregateCommandParameter : CommandParameter
    {
        /// <summary>
        /// Gets the parameters which constitute this command parameter.
        /// </summary>
        public IReadOnlyList<CommandParameter> Parameters { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="parameters">The parameters which constitute this command parameter.</param>
        public AggregateCommandParameter(IReadOnlyList<CommandParameter> parameters)
        {
            Parameters = parameters ?? Enumerable.Empty<CommandParameter>().ToList();
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || obj.GetType() != typeof(AggregateCommandParameter))
                return false;
            
            var other = (AggregateCommandParameter)obj;
            
            if(Parameters.Count != other.Parameters.Count)
                return false;

            for(var i = 0; i < Parameters.Count; i++)
            {
                if(!Parameters[i].Equals(other.Parameters[i]))
                    return false;
            }

            return true;
        }
        
        public override int GetHashCode()
        {
            var value = 0;
            foreach(var parameter in Parameters)
            {
                value += parameter.GetHashCode();
            }

            return value;
        }
    }
}