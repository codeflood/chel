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
                return ApplicationTextResolver.Instance.Resolve(ApplicationTexts.InvalidCommandNameNull);

            var text = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.InvalidCommandNameWithParameter);
            return string.Format(text, CommandName);
        }
    }
}