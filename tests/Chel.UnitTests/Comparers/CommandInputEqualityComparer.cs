using System.Collections.Generic;
using Chel.Abstractions.Parsing;

namespace Chel.UnitTests.Comparers
{
	public class CommandInputEqualityComparer : IEqualityComparer<CommandInput>
    {
        public bool Equals(CommandInput x, CommandInput y)
        {
            var parametersEqual = true;

            if(!x.Parameters.Count.Equals(y.Parameters.Count))
                return false;

            for(var i = 0; i < x.Parameters.Count; i++)
            {
                if(x.Parameters[i] is CommandInput)
                    parametersEqual = Equals((CommandInput)x.Parameters[i], (CommandInput)y.Parameters[i]);
                else
                    parametersEqual = x.Parameters[i].Equals(y.Parameters[i]);

                if(!parametersEqual)
                    break;
            }

            return
                x.CommandName.Equals(y.CommandName) &&
                x.SourceLine.Equals(y.SourceLine) &&
                parametersEqual;
        }

        public int GetHashCode(CommandInput obj)
        {
            var hashCode = 0;

            foreach(var p in obj.Parameters)
                hashCode += p.GetHashCode();

            return
                obj.CommandName.GetHashCode() +
                obj.SourceLine.GetHashCode() +
                hashCode;
        }
    }
}