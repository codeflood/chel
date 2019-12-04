using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Generates command descriptors.
    /// </summary>
    public interface ICommandDescriptorGenerator
    {
        /// <summary>
        /// Describe a command.
        /// </summary>
        /// <param name="commandType">The type implementing the command.</param>
        CommandDescriptor DescribeCommand(Type commandType);
    }
}