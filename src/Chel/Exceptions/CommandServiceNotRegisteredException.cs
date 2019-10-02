using System;

namespace Chel
{
    public class CommandServiceNotRegisteredException : Exception
    {
        public Type CommandServiceType { get; private set; }

        public CommandServiceNotRegisteredException(Type type)
            : base($"Command service implementing {type.Name} has not been registered")
        {
            CommandServiceType = type;
        }
    }
}