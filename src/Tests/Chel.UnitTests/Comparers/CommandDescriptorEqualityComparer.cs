using System.Collections.Generic;
using Chel.Abstractions;

namespace Chel.UnitTests.Comparers
{
    public class CommandDescriptorEqualityComparer : IEqualityComparer<CommandDescriptor>
    {
        public bool Equals(CommandDescriptor x, CommandDescriptor y)
        {
            return
                x.ImplementingType.Equals(y.ImplementingType) &&
                string.Compare(x.CommandName, y.CommandName, true) == 0;
        }

        public int GetHashCode(CommandDescriptor obj)
        {
            return obj.ImplementingType.GetHashCode() + obj.CommandName.ToLower().GetHashCode();
        }
    }
}