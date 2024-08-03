using System.Collections.Generic;
using Chel.Abstractions;

namespace Chel.UnitTests.Comparers
{
    public class CommandDescriptorEqualityComparer : IEqualityComparer<CommandDescriptor?>
    {
        public bool Equals(CommandDescriptor? x, CommandDescriptor? y)
        {
            if(x == null || y == null)
                return false;

            return
                x.ImplementingType.Equals(y.ImplementingType) &&
                x.CommandIdentifier.Equals(y.CommandIdentifier);
        }

        public int GetHashCode(CommandDescriptor obj)
        {
            return obj.ImplementingType.GetHashCode() + obj.CommandIdentifier.GetHashCode();
        }
    }
}