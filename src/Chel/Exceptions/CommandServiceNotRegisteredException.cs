using System;

namespace Chel
{
    public class CommandServiceNotRegisteredException : Exception
    {
        public Type CommandServiceType { get; private set; }

        public CommandServiceNotRegisteredException(Type type)
            : base(string.Format(Texts.CommandServiceNotRegistered, type?.FullName))
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            CommandServiceType = type;
        }
    }
}