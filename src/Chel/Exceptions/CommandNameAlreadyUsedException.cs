using System;

namespace Chel
{
    public class CommandNameAlreadyUsedException : Exception
    {
        public string CommandName { get; private set; }

        public Type CommandType { get; private set; }

        public Type OtherCommandType { get; private set; }

        public CommandNameAlreadyUsedException(string commandName, Type commandType, Type otherCommandType)
            : base(string.Format(Texts.CommandNameAlreadyUsed, commandName, commandType?.FullName, otherCommandType?.FullName))
        {
            if(commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if(commandType == null)
                throw new ArgumentNullException(nameof(commandType));

            if(otherCommandType == null)
                throw new ArgumentNullException(nameof(otherCommandType));

            CommandName = commandName;
            CommandType = commandType;
            OtherCommandType = otherCommandType;
        }
    }
}