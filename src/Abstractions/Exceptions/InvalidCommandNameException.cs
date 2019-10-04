using System;

namespace Chel.Abstractions
{
    public class InvalidCommandNameException : Exception
    {
        public string CommandName { get; private set; }

        public InvalidCommandNameException(string commandName)
        {
            CommandName = commandName;
        }

        public override string ToString()
        {
            if(CommandName == null)
                return Texts.InvalidCommandNameNull;

            return string.Format(Texts.InvalidCommandNameWithParameter, CommandName);
        }
    }
}